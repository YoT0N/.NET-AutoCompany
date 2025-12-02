using RoutingService.Bll.DTOs.Common;
using RoutingService.Bll.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Application.Interfaces
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto> GetRouteByIdAsync(int id);
        Task<RouteWithStopsDto> GetRouteWithStopsAsync(int id);
        Task<RouteDto> CreateRouteAsync(CreateRouteDto dto);
        Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto dto);
        Task DeleteRouteAsync(int id);
        Task<bool> RouteExistsAsync(int id);

        Task<PagedResultDto<RouteDto>> GetRoutesPagedAsync(RouteFilterParameters parameters);
    }
}