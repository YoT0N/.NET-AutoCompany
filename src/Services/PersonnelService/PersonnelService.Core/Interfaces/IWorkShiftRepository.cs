using PersonnelService.Domain.Entities;

namespace PersonnelService.Domain.Interfaces
{
    public interface IWorkShiftRepository
    {
        Task<WorkShiftLog?> GetByIdAsync(string id);
        Task<IReadOnlyCollection<WorkShiftLog>> GetAllAsync();
        Task<IReadOnlyCollection<WorkShiftLog>> GetByPersonnelIdAsync(int personnelId);
        Task<IReadOnlyCollection<WorkShiftLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IReadOnlyCollection<WorkShiftLog>> GetByPersonnelAndDateRangeAsync(
            int personnelId,
            DateTime startDate,
            DateTime endDate);
        Task<IReadOnlyCollection<WorkShiftLog>> GetByBusNumberAsync(string busCountryNumber);
        Task<IReadOnlyCollection<WorkShiftLog>> GetByRouteNumberAsync(string routeNumber);
        Task<IReadOnlyCollection<WorkShiftLog>> GetByStatusAsync(string status);

        Task AddAsync(WorkShiftLog workShift);
        Task UpdateAsync(WorkShiftLog workShift);
        Task DeleteAsync(string id);

        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

        // Aggregation methods
        Task<double> GetTotalDistanceByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate);
        Task<int> GetShiftCountByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate);
        Task<IReadOnlyCollection<WorkShiftLog>> GetUpcomingShiftsAsync(int personnelId, int daysAhead = 7);
    }
}