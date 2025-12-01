using AutoMapper;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Bll.DTOs.Examination;
using TechnicalService.Domain.Entities;
using TechnicalService.Domain.Exceptions;
using TechnicalService.Bll.Interfaces;

namespace TechnicalService.Bll.Services;

/// <summary>
/// Сервіс для роботи з технічними оглядами автобусів
/// Містить бізнес-логіку, валідацію та координацію транзакцій
/// </summary>
public class ExaminationService : IExaminationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExaminationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ExaminationDto>> GetAllExaminationsAsync(
        CancellationToken cancellationToken = default)
    {
        var examinations = await _unitOfWork.Examinations
            .GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<ExaminationDto>>(examinations);
    }

    public async Task<ExaminationDto> GetExaminationByIdAsync(
        long examinationId,
        CancellationToken cancellationToken = default)
    {
        var examination = await _unitOfWork.Examinations
            .GetByIdAsync(examinationId, cancellationToken);

        if (examination == null)
        {
            throw new NotFoundException($"Examination with ID {examinationId} not found");
        }

        return _mapper.Map<ExaminationDto>(examination);
    }

    public async Task<IEnumerable<ExaminationDto>> GetExaminationsByBusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        // Валідація: чи існує автобус
        var bus = await _unitOfWork.Buses
            .GetByIdAsync(countryNumber, cancellationToken);

        if (bus == null)
        {
            throw new NotFoundException($"Bus with country number {countryNumber} not found");
        }

        var examinations = await _unitOfWork.Examinations
            .GetExaminationsByBusAsync(countryNumber, cancellationToken);

        return _mapper.Map<IEnumerable<ExaminationDto>>(examinations);
    }

    public async Task<IEnumerable<ExaminationDto>> GetFailedExaminationsAsync(
        CancellationToken cancellationToken = default)
    {
        var examinations = await _unitOfWork.Examinations
            .GetFailedExaminationsAsync(cancellationToken);

        return _mapper.Map<IEnumerable<ExaminationDto>>(examinations);
    }

    public async Task<ExaminationDto> GetExaminationWithPartsAsync(
        long examinationId,
        CancellationToken cancellationToken = default)
    {
        var examination = await _unitOfWork.Examinations
            .GetExaminationWithPartsAsync(examinationId, cancellationToken);

        if (examination == null)
        {
            throw new NotFoundException($"Examination with ID {examinationId} not found");
        }

        return _mapper.Map<ExaminationDto>(examination);
    }

    /// <summary>
    /// Створення огляду з запчастинами - ТРАНЗАКЦІЙНА ОПЕРАЦІЯ
    /// Координує кілька операцій: створення огляду, додавання запчастин, оновлення статусу автобуса
    /// </summary>
    public async Task<long> CreateExaminationAsync(
        CreateExaminationDto dto,
        CancellationToken cancellationToken = default)
    {
        // 1. ВАЛІДАЦІЯ: чи існує автобус
        var bus = await _unitOfWork.Buses
            .GetByIdAsync(dto.BusCountryNumber, cancellationToken);

        if (bus == null)
        {
            throw new NotFoundException(
                $"Bus with country number {dto.BusCountryNumber} not found");
        }

        // 2. ВАЛІДАЦІЯ: чи існують всі запчастини
        if (dto.RepairParts?.Any() == true)
        {
            foreach (var partDto in dto.RepairParts)
            {
                var repairPart = await _unitOfWork.RepairParts
                    .GetByIdAsync(partDto.PartId, cancellationToken);

                if (repairPart == null)
                {
                    throw new NotFoundException(
                        $"Repair part with ID {partDto.PartId} not found");
                }

                // БІЗНЕС-ПРАВИЛО: перевірка наявності запчастин на складі
                if (repairPart.StockQuantity < partDto.Quantity)
                {
                    throw new BusinessConflictException(
                        $"Insufficient stock for part '{repairPart.PartName}'. " +
                        $"Available: {repairPart.StockQuantity}, Required: {partDto.Quantity}");
                }
            }
        }

        // 3. БІЗНЕС-ПРАВИЛО: розрахунок RepairPrice на основі запчастин
        decimal calculatedRepairPrice = 0;
        if (dto.RepairParts?.Any() == true)
        {
            foreach (var partDto in dto.RepairParts)
            {
                var repairPart = await _unitOfWork.RepairParts
                    .GetByIdAsync(partDto.PartId, cancellationToken);

                calculatedRepairPrice += (repairPart?.UnitPrice ?? 0) * partDto.Quantity;
            }
        }

        // Якщо користувач не вказав ціну або вказав 0 - використовуємо розраховану
        if (dto.RepairPrice == 0 && calculatedRepairPrice > 0)
        {
            dto.RepairPrice = calculatedRepairPrice;
        }

        // 4. ПОЧАТОК ТРАНЗАКЦІЇ
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var examination = _mapper.Map<TechnicalExamination>(dto);
            var parts = _mapper.Map<List<ExaminationRepairPart>>(dto.RepairParts);

            // 5. Створення огляду з запчастинами
            var examinationId = await _unitOfWork.Examinations
                .CreateExaminationWithPartsAsync(
                    examination,
                    parts,
                    _unitOfWork.Connection!,
                    _unitOfWork.Transaction!,
                    cancellationToken);

            // 6. Зменшення кількості запчастин на складі
            if (dto.RepairParts?.Any() == true)
            {
                foreach (var partDto in dto.RepairParts)
                {
                    await _unitOfWork.RepairParts.UpdateStockQuantityAsync(
                        partDto.PartId,
                        -partDto.Quantity,  // ← Від'ємне значення = зменшення
                        cancellationToken);
                }
            }

            // 7. Оновлення статусу автобуса (якщо відправлений на ремонт)
            if (dto.SentForRepair)
            {
                // Припустимо: StatusId = 3 означає "В ремонті"
                // Це має бути в константах або конфігурації
                const int InRepairStatusId = 3;

                await _unitOfWork.Buses.UpdateBusStatusAsync(
                    dto.BusCountryNumber,
                    InRepairStatusId,
                    cancellationToken);
            }

            // 8. COMMIT транзакції
            await _unitOfWork.CommitAsync(cancellationToken);

            return examinationId;
        }
        catch
        {
            // 9. ROLLBACK при помилці
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<int> UpdateExaminationAsync(
        long examinationId,
        UpdateExaminationDto dto,
        CancellationToken cancellationToken = default)
    {
        // 1. ВАЛІДАЦІЯ: чи існує огляд
        var existingExamination = await _unitOfWork.Examinations
            .GetByIdAsync(examinationId, cancellationToken);

        if (existingExamination == null)
        {
            throw new NotFoundException($"Examination with ID {examinationId} not found");
        }

        // 2. ВАЛІДАЦІЯ: чи існує автобус (якщо змінюється)
        if (dto.BusCountryNumber != existingExamination.BusCountryNumber)
        {
            var bus = await _unitOfWork.Buses
                .GetByIdAsync(dto.BusCountryNumber, cancellationToken);

            if (bus == null)
            {
                throw new NotFoundException(
                    $"Bus with country number {dto.BusCountryNumber} not found");
            }
        }

        // 3. Мапінг змін
        _mapper.Map(dto, existingExamination);

        // 4. Оновлення
        return await _unitOfWork.Examinations
            .UpdateAsync(existingExamination, cancellationToken);
    }

    public async Task<int> DeleteExaminationAsync(
        long examinationId,
        CancellationToken cancellationToken = default)
    {
        // ВАЛІДАЦІЯ: чи існує огляд
        var examination = await _unitOfWork.Examinations
            .GetByIdAsync(examinationId, cancellationToken);

        if (examination == null)
        {
            throw new NotFoundException($"Examination with ID {examinationId} not found");
        }

        // БІЗНЕС-ПРАВИЛО: можна видалити тільки огляди старше 30 днів
        // (приклад бізнес-логіки)
        if (examination.ExaminationDate > DateTime.UtcNow.AddDays(-30))
        {
            throw new BusinessConflictException(
                "Cannot delete examination created within the last 30 days");
        }

        return await _unitOfWork.Examinations
            .DeleteAsync(examinationId, cancellationToken);
    }
}