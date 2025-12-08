using System.Collections.Generic;
using System.Threading.Tasks;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.DTOs;

namespace RoutingService.Bll.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto?> GetScheduleByIdAsync(int id);
        Task<IEnumerable<ScheduleWithRouteDto>> GetSchedulesWithRouteInfoAsync();
        Task<IEnumerable<ScheduleDto>> GetSchedulesByRouteAsync(int routeId);
        Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto dto);
        Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto dto);
        Task<bool> DeleteScheduleAsync(int id);

        Task<PagedResultDto<ScheduleWithRouteDto>> GetSchedulesPagedAsync(ScheduleFilterParameters parameters);
    }
}