using TechnicalService.Application.Interfaces;
using TechnicalService.Core.DTOs;
using TechnicalService.Core.Entities;
using TechnicalService.Core.Interfaces;

namespace TechnicalService.Application.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IUnitOfWork _unitOfWork;

    public MaintenanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MaintenanceHistoryDto>> GetAllMaintenanceAsync()
    {
        var maintenanceRecords = await _unitOfWork.Maintenance.GetAllAsync();
        return maintenanceRecords.Select(MapToDto);
    }

    public async Task<MaintenanceHistoryDto?> GetMaintenanceByIdAsync(long maintenanceId)
    {
        var maintenance = await _unitOfWork.Maintenance.GetByIdAsync(maintenanceId);
        return maintenance != null ? MapToDto(maintenance) : null;
    }

    public async Task<IEnumerable<MaintenanceHistoryDto>> GetMaintenanceByBusAsync(string countryNumber)
    {
        var maintenanceRecords = await _unitOfWork.Maintenance.GetMaintenanceByBusAsync(countryNumber);
        return maintenanceRecords.Select(MapToDto);
    }

    public async Task<IEnumerable<MaintenanceHistoryDto>> GetUpcomingMaintenanceAsync(DateTime fromDate)
    {
        var maintenanceRecords = await _unitOfWork.Maintenance.GetUpcomingMaintenanceAsync(fromDate);
        return maintenanceRecords.Select(MapToDto);
    }

    public async Task<decimal> GetTotalMaintenanceCostAsync(string countryNumber)
    {
        return await _unitOfWork.Maintenance.GetTotalMaintenanceCostAsync(countryNumber);
    }

    public async Task<int> CreateMaintenanceAsync(CreateMaintenanceDto createMaintenanceDto)
    {
        var maintenance = new BusMaintenanceHistory
        {
            BusCountryNumber = createMaintenanceDto.BusCountryNumber,
            MaintenanceDate = createMaintenanceDto.MaintenanceDate,
            MaintenanceType = createMaintenanceDto.MaintenanceType,
            Description = createMaintenanceDto.Description,
            Cost = createMaintenanceDto.Cost,
            MechanicName = createMaintenanceDto.MechanicName,
            NextMaintenanceDate = createMaintenanceDto.NextMaintenanceDate
        };

        return await _unitOfWork.Maintenance.AddAsync(maintenance);
    }

    public async Task<int> UpdateMaintenanceAsync(long maintenanceId, CreateMaintenanceDto updateMaintenanceDto)
    {
        var existingMaintenance = await _unitOfWork.Maintenance.GetByIdAsync(maintenanceId);
        if (existingMaintenance == null)
        {
            return 0;
        }

        existingMaintenance.MaintenanceDate = updateMaintenanceDto.MaintenanceDate;
        existingMaintenance.MaintenanceType = updateMaintenanceDto.MaintenanceType;
        existingMaintenance.Description = updateMaintenanceDto.Description;
        existingMaintenance.Cost = updateMaintenanceDto.Cost;
        existingMaintenance.MechanicName = updateMaintenanceDto.MechanicName;
        existingMaintenance.NextMaintenanceDate = updateMaintenanceDto.NextMaintenanceDate;

        return await _unitOfWork.Maintenance.UpdateAsync(existingMaintenance);
    }

    public async Task<int> DeleteMaintenanceAsync(long maintenanceId)
    {
        return await _unitOfWork.Maintenance.DeleteAsync(maintenanceId);
    }

    private static MaintenanceHistoryDto MapToDto(BusMaintenanceHistory maintenance)
    {
        return new MaintenanceHistoryDto
        {
            MaintenanceId = maintenance.MaintenanceId,
            BusCountryNumber = maintenance.BusCountryNumber,
            MaintenanceDate = maintenance.MaintenanceDate,
            MaintenanceType = maintenance.MaintenanceType,
            Description = maintenance.Description,
            Cost = maintenance.Cost,
            MechanicName = maintenance.MechanicName,
            NextMaintenanceDate = maintenance.NextMaintenanceDate
        };
    }
}