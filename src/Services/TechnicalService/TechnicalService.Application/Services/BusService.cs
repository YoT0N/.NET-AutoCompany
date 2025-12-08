using AutoMapper;
using TechnicalService.Bll.DTOs.Bus;
using TechnicalService.Bll.Interfaces;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Domain.Entities;
using TechnicalService.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace TechnicalService.Bll.Services;

public class BusService : IBusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BusService> _logger;

    public BusService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<BusService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BusDto>> GetAllBusesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отримання всіх автобусів");

        var buses = await _unitOfWork.Buses.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<BusDto>>(buses);
    }

    public async Task<BusDto> GetBusByCountryNumberAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отримання автобуса з номером {CountryNumber}", countryNumber);

        var bus = await _unitOfWork.Buses.GetByIdAsync(countryNumber, cancellationToken);

        if (bus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return _mapper.Map<BusDto>(bus);
    }

    public async Task<BusDto> GetBusWithStatusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отримання автобуса зі статусом {CountryNumber}", countryNumber);

        var bus = await _unitOfWork.Buses.GetBusWithStatusAsync(countryNumber, cancellationToken);

        if (bus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return _mapper.Map<BusDto>(bus);
    }

    public async Task<IEnumerable<BusDto>> GetBusesByStatusAsync(
        int statusId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отримання автобусів зі статусом {StatusId}", statusId);

        var buses = await _unitOfWork.Buses.GetBusesByStatusAsync(statusId, cancellationToken);
        return _mapper.Map<IEnumerable<BusDto>>(buses);
    }

    public async Task<IEnumerable<BusDto>> GetActiveBusesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отримання активних автобусів");

        var buses = await _unitOfWork.Buses.GetActiveBusesAsync(cancellationToken);
        return _mapper.Map<IEnumerable<BusDto>>(buses);
    }

    public async Task<string> CreateBusAsync(
        CreateBusDto createBusDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Створення нового автобуса {CountryNumber}", createBusDto.CountryNumber);

        // Бізнес-валідація: перевірка унікальності номера
        var existingBus = await _unitOfWork.Buses.GetByIdAsync(
            createBusDto.CountryNumber,
            cancellationToken);

        if (existingBus != null)
        {
            throw new BusinessConflictException(
                $"Автобус з номером {createBusDto.CountryNumber} вже існує");
        }

        // Бізнес-валідація: рік випуску
        if (createBusDto.YearOfManufacture > DateTime.UtcNow.Year)
        {
            throw new ValidationException(
                nameof(createBusDto.YearOfManufacture),
                "Рік випуску не може бути в майбутньому");
        }

        var bus = _mapper.Map<Bus>(createBusDto);
        bus.CreatedAt = DateTime.UtcNow;
        bus.UpdatedAt = DateTime.UtcNow;
        bus.IsDeleted = false;

        await _unitOfWork.Buses.AddAsync(bus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Автобус {CountryNumber} успішно створено", bus.CountryNumber);

        return bus.CountryNumber;
    }

    public async Task UpdateBusAsync(
        string countryNumber,
        UpdateBusDto updateBusDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Оновлення автобуса {CountryNumber}", countryNumber);

        var existingBus = await _unitOfWork.Buses.GetByIdAsync(countryNumber, cancellationToken);

        if (existingBus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        // Використання AutoMapper для оновлення
        _mapper.Map(updateBusDto, existingBus);
        existingBus.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Buses.UpdateAsync(existingBus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Автобус {CountryNumber} успішно оновлено", countryNumber);
    }

    public async Task DeleteBusAsync(string countryNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Видалення автобуса {CountryNumber}", countryNumber);

        var existingBus = await _unitOfWork.Buses.GetByIdAsync(countryNumber, cancellationToken);

        if (existingBus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        // Бізнес-правило: не можна видаляти автобус, який на ремонті
        if (existingBus.CurrentStatusId == 3) // UnderRepair
        {
            throw new BusinessConflictException(
                "Неможливо видалити автобус, який знаходиться на ремонті");
        }

        await _unitOfWork.Buses.DeleteAsync(countryNumber, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Автобус {CountryNumber} успішно видалено", countryNumber);
    }

    public async Task UpdateBusStatusAsync(
        string countryNumber,
        int newStatusId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Оновлення статусу автобуса {CountryNumber} на {StatusId}",
            countryNumber,
            newStatusId);

        var existingBus = await _unitOfWork.Buses.GetByIdAsync(countryNumber, cancellationToken);

        if (existingBus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        // Бізнес-валідація статусів (приклад)
        if (newStatusId < 1 || newStatusId > 5)
        {
            throw new ValidationException("newStatusId", "Невірний ідентифікатор статусу");
        }

        await _unitOfWork.Buses.UpdateBusStatusAsync(countryNumber, newStatusId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Статус автобуса {CountryNumber} успішно оновлено", countryNumber);
    }

    public async Task<decimal> GetTotalMileageAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Отримання пробігу автобуса {CountryNumber}", countryNumber);

        var bus = await _unitOfWork.Buses.GetByIdAsync(countryNumber, cancellationToken);

        if (bus == null)
        {
            throw new NotFoundException("Bus", countryNumber);
        }

        return await _unitOfWork.Buses.GetTotalMileageAsync(countryNumber, cancellationToken);
    }
}