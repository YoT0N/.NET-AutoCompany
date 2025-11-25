using PersonnelService.Core.DTOs;

namespace PersonnelService.Application.Interfaces
{
    public interface IWorkShiftService
    {
        Task<IEnumerable<WorkShiftDto>> GetAllWorkShiftsAsync();
        Task<WorkShiftDto?> GetWorkShiftByIdAsync(string id);
        Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByPersonnelAndDateRangeAsync(int personnelId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByBusNumberAsync(string busCountryNumber);
        Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByRouteNumberAsync(string routeNumber);
        Task<IEnumerable<WorkShiftDto>> GetWorkShiftsByStatusAsync(string status);
        Task<WorkShiftDto> CreateWorkShiftAsync(CreateWorkShiftDto createDto);
        Task<bool> UpdateWorkShiftAsync(string id, UpdateWorkShiftDto updateDto);
        Task<bool> DeleteWorkShiftAsync(string id);
        Task<bool> DeleteWorkShiftsByPersonnelIdAsync(int personnelId);
        Task<bool> UpdateWorkShiftStatusAsync(string id, string status);
        Task<double> GetTotalDistanceByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate);
        Task<int> GetShiftCountByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate);
    }
}