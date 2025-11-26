using TechnicalService.Bll.DTOs.Maintenance;

namespace TechnicalService.Bll.Interfaces;

public interface IMaintenanceService
{
    Task<IEnumerable<MaintenanceHistoryDto>> GetAllMaintenanceAsync();
    Task<MaintenanceHistoryDto?> GetMaintenanceByIdAsync(long maintenanceId);
    Task<IEnumerable<MaintenanceHistoryDto>> GetMaintenanceByBusAsync(string countryNumber);
    Task<IEnumerable<MaintenanceHistoryDto>> GetUpcomingMaintenanceAsync(DateTime fromDate);
    Task<decimal> GetTotalMaintenanceCostAsync(string countryNumber);
    Task<int> CreateMaintenanceAsync(CreateMaintenanceDto createMaintenanceDto);
    Task<int> UpdateMaintenanceAsync(long maintenanceId, CreateMaintenanceDto updateMaintenanceDto);
    Task<int> DeleteMaintenanceAsync(long maintenanceId);
}