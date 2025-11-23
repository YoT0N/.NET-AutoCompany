using TechnicalService.Application.Interfaces;
using TechnicalService.Core.DTOs;
using TechnicalService.Core.Entities;
using TechnicalService.Core.Interfaces;

namespace TechnicalService.Application.Services;

public class ExaminationService : IExaminationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExaminationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ExaminationDto>> GetAllExaminationsAsync()
    {
        var examinations = await _unitOfWork.Examinations.GetAllAsync();
        return examinations.Select(MapToDto);
    }

    public async Task<ExaminationDto?> GetExaminationByIdAsync(long examinationId)
    {
        var examination = await _unitOfWork.Examinations.GetByIdAsync(examinationId);
        return examination != null ? MapToDto(examination) : null;
    }

    public async Task<IEnumerable<ExaminationDto>> GetExaminationsByBusAsync(string countryNumber)
    {
        var examinations = await _unitOfWork.Examinations.GetExaminationsByBusAsync(countryNumber);
        return examinations.Select(MapToDto);
    }

    public async Task<IEnumerable<ExaminationDto>> GetFailedExaminationsAsync()
    {
        var examinations = await _unitOfWork.Examinations.GetFailedExaminationsAsync();
        return examinations.Select(MapToDto);
    }

    public async Task<ExaminationDto?> GetExaminationWithPartsAsync(long examinationId)
    {
        var examination = await _unitOfWork.Examinations.GetExaminationWithPartsAsync(examinationId);
        return examination != null ? MapToDtoWithParts(examination) : null;
    }

    public async Task<long> CreateExaminationAsync(CreateExaminationDto createExaminationDto)
    {
        var examination = new TechnicalExamination
        {
            BusCountryNumber = createExaminationDto.BusCountryNumber,
            ExaminationDate = createExaminationDto.ExaminationDate,
            ExaminationResult = createExaminationDto.ExaminationResult,
            SentForRepair = createExaminationDto.SentForRepair,
            RepairPrice = createExaminationDto.RepairPrice,
            MechanicName = createExaminationDto.MechanicName,
            Notes = createExaminationDto.Notes
        };

        var parts = createExaminationDto.RepairParts?.Select(p => new ExaminationRepairPart
        {
            PartId = p.PartId,
            Quantity = p.Quantity
        }).ToList() ?? new List<ExaminationRepairPart>();

        return await _unitOfWork.Examinations.CreateExaminationWithPartsAsync(examination, parts);
    }

    public async Task<int> UpdateExaminationAsync(long examinationId, CreateExaminationDto updateExaminationDto)
    {
        var existingExamination = await _unitOfWork.Examinations.GetByIdAsync(examinationId);
        if (existingExamination == null)
        {
            return 0;
        }

        existingExamination.ExaminationDate = updateExaminationDto.ExaminationDate;
        existingExamination.ExaminationResult = updateExaminationDto.ExaminationResult;
        existingExamination.SentForRepair = updateExaminationDto.SentForRepair;
        existingExamination.RepairPrice = updateExaminationDto.RepairPrice;
        existingExamination.MechanicName = updateExaminationDto.MechanicName;
        existingExamination.Notes = updateExaminationDto.Notes;

        return await _unitOfWork.Examinations.UpdateAsync(existingExamination);
    }

    public async Task<int> DeleteExaminationAsync(long examinationId)
    {
        return await _unitOfWork.Examinations.DeleteAsync(examinationId);
    }

    private static ExaminationDto MapToDto(TechnicalExamination examination)
    {
        return new ExaminationDto
        {
            ExaminationId = examination.ExaminationId,
            BusCountryNumber = examination.BusCountryNumber,
            ExaminationDate = examination.ExaminationDate,
            ExaminationResult = examination.ExaminationResult,
            SentForRepair = examination.SentForRepair,
            RepairPrice = examination.RepairPrice,
            MechanicName = examination.MechanicName,
            Notes = examination.Notes,
            RepairParts = new List<RepairPartDto>()
        };
    }

    private static ExaminationDto MapToDtoWithParts(TechnicalExamination examination)
    {
        return new ExaminationDto
        {
            ExaminationId = examination.ExaminationId,
            BusCountryNumber = examination.BusCountryNumber,
            ExaminationDate = examination.ExaminationDate,
            ExaminationResult = examination.ExaminationResult,
            SentForRepair = examination.SentForRepair,
            RepairPrice = examination.RepairPrice,
            MechanicName = examination.MechanicName,
            Notes = examination.Notes,
            RepairParts = examination.RepairParts?.Select(rp => new RepairPartDto
            {
                PartId = rp.PartId,
                PartName = rp.Part?.PartName ?? string.Empty,
                Quantity = rp.Quantity,
                UnitPrice = rp.Part?.UnitPrice ?? 0,
                TotalPrice = rp.Quantity * (rp.Part?.UnitPrice ?? 0)
            }).ToList() ?? new List<RepairPartDto>()
        };
    }
}