using System.Collections.Generic;
using System.Threading.Tasks;
using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.DTOs;

namespace RoutingService.Bll.Interfaces
{
    public interface IRouteStopService
    {
        Task<IEnumerable<RouteStopDto>> GetAllStopsAsync();
        Task<RouteStopDto?> GetStopByIdAsync(int id);
        Task<RouteStopDto> CreateStopAsync(CreateRouteStopDto dto);
        Task<RouteStopDto?> UpdateStopAsync(int id, UpdateRouteStopDto dto);
        Task<bool> DeleteStopAsync(int id);
        Task<bool> AssignStopToRouteAsync(AssignStopToRouteDto dto);
        Task<bool> RemoveStopFromRouteAsync(int routeId, int stopId);
        Task<IEnumerable<RouteStopInfoDto>> GetStopsByRouteAsync(int routeId);

        Task<PagedResultDto<RouteStopDto>> GetStopsPagedAsync(RouteStopFilterParameters parameters);
    }
}