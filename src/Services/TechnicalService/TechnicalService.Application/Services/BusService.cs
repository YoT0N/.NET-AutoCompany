using TechnicalService.Application.Interfaces;
using TechnicalService.Core.DTOs;
using TechnicalService.Core.Entities;
using TechnicalService.Core.Interfaces;

namespace TechnicalService.Application.Services;

public class BusService : IBusService
{
    private readonly IUnitOfWork _unitOfWork;

    public BusService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BusDto>> GetAllBusesAsync()
    {
        var buses = await _unitOfWork.Buses.GetAllAsync();
        return buses.Select(MapToDto);
    }

    public async Task<BusDto?> GetBusByCountryNumberAsync(string countryNumber)
    {
        var bus = await _unitOfWork.Buses.GetByIdAsync(countryNumber);
        return bus != null ? MapToDto(bus) : null;
    }

    public async Task<BusDto?> GetBusWithStatusAsync(string countryNumber)
    {
        var bus = await _unitOfWork.Buses.GetBusWithStatusAsync(countryNumber);
        return bus != null ? MapToDtoWithStatus(bus) : null;
    }

    public async Task<IEnumerable<BusDto>> GetBusesByStatusAsync(int statusId)
    {
        var buses = await _unitOfWork.Buses.GetBusesByStatusAsync(statusId);
        return buses.Select(MapToDto);
    }

    public async Task<IEnumerable<BusDto>> GetActiveBusesAsync()
    {
        var buses = await _unitOfWork.Buses.GetActiveBusesAsync();
        return buses.Select(MapToDtoWithStatus);
    }

    public async Task<int> CreateBusAsync(CreateBusDto createBusDto)
    {
        var bus = new Bus
        {
            CountryNumber = createBusDto.CountryNumber,
            BoardingNumber = createBusDto.BoardingNumber,
            Brand = createBusDto.Brand,
            PassengerCapacity = createBusDto.PassengerCapacity,
            YearOfManufacture = createBusDto.YearOfManufacture,
            Mileage = createBusDto.Mileage,
            DateOfReceipt = createBusDto.DateOfReceipt,
            CurrentStatusId = createBusDto.CurrentStatusId
        };

        return await _unitOfWork.Buses.AddAsync(bus);
    }

    public async Task<int> UpdateBusAsync(string countryNumber, UpdateBusDto updateBusDto)
    {
        var existingBus = await _unitOfWork.Buses.GetByIdAsync(countryNumber);
        if (existingBus == null)
        {
            return 0;
        }

        if (updateBusDto.BoardingNumber != null)
            existingBus.BoardingNumber = updateBusDto.BoardingNumber;

        if (updateBusDto.Brand != null)
            existingBus.Brand = updateBusDto.Brand;

        if (updateBusDto.PassengerCapacity.HasValue)
            existingBus.PassengerCapacity = updateBusDto.PassengerCapacity.Value;

        if (updateBusDto.Mileage.HasValue)
            existingBus.Mileage = updateBusDto.Mileage.Value;

        if (updateBusDto.CurrentStatusId.HasValue)
            existingBus.CurrentStatusId = updateBusDto.CurrentStatusId.Value;

        if (updateBusDto.WriteoffDate.HasValue)
            existingBus.WriteoffDate = updateBusDto.WriteoffDate;

        return await _unitOfWork.Buses.UpdateAsync(existingBus);
    }

    public async Task<int> DeleteBusAsync(string countryNumber)
    {
        return await _unitOfWork.Buses.DeleteAsync(countryNumber);
    }

    public async Task<int> UpdateBusStatusAsync(string countryNumber, int newStatusId)
    {
        return await _unitOfWork.Buses.UpdateBusStatusAsync(countryNumber, newStatusId);
    }

    public async Task<decimal> GetTotalMileageAsync(string countryNumber)
    {
        return await _unitOfWork.Buses.GetTotalMileageAsync(countryNumber);
    }

    private static BusDto MapToDto(Bus bus)
    {
        return new BusDto
        {
            CountryNumber = bus.CountryNumber,
            BoardingNumber = bus.BoardingNumber,
            Brand = bus.Brand,
            PassengerCapacity = bus.PassengerCapacity,
            YearOfManufacture = bus.YearOfManufacture,
            Mileage = bus.Mileage,
            DateOfReceipt = bus.DateOfReceipt,
            WriteoffDate = bus.WriteoffDate,
            CurrentStatusId = bus.CurrentStatusId
        };
    }

    private static BusDto MapToDtoWithStatus(Bus bus)
    {
        return new BusDto
        {
            CountryNumber = bus.CountryNumber,
            BoardingNumber = bus.BoardingNumber,
            Brand = bus.Brand,
            PassengerCapacity = bus.PassengerCapacity,
            YearOfManufacture = bus.YearOfManufacture,
            Mileage = bus.Mileage,
            DateOfReceipt = bus.DateOfReceipt,
            WriteoffDate = bus.WriteoffDate,
            CurrentStatusId = bus.CurrentStatusId,
            CurrentStatusName = bus.CurrentStatus?.StatusName
        };
    }
}