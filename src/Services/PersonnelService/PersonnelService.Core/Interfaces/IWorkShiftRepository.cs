using PersonnelService.Core.Models;

namespace PersonnelService.Core.Interfaces
{
    public interface IWorkShiftRepository
    {
        Task<IEnumerable<WorkShiftLog>> GetAllAsync();
        Task<WorkShiftLog?> GetByIdAsync(string id);
        Task<IEnumerable<WorkShiftLog>> GetByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<WorkShiftLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<WorkShiftLog>> GetByPersonnelAndDateRangeAsync(int personnelId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<WorkShiftLog>> GetByBusNumberAsync(string busCountryNumber);
        Task<IEnumerable<WorkShiftLog>> GetByRouteNumberAsync(string routeNumber);
        Task<IEnumerable<WorkShiftLog>> GetByStatusAsync(string status);
        Task<WorkShiftLog> CreateAsync(WorkShiftLog workShift);
        Task<bool> UpdateAsync(string id, WorkShiftLog workShift);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByPersonnelIdAsync(int personnelId);
        Task<bool> UpdateStatusAsync(string id, string status);
        Task<double> GetTotalDistanceByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate);
        Task<int> GetShiftCountByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate);
    }
}