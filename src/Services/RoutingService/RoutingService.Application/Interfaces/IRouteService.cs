using System.Collections.Generic;
using System.Threading.Tasks;
using RoutingService.Core.DTOs;

namespace RoutingService.Application.Interfaces
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto?> GetRouteByIdAsync(int id);
        Task<RouteWithStopsDto?> GetRouteWithStopsAsync(int id);
        Task<RouteDto> CreateRouteAsync(CreateRouteDto dto);
        Task<RouteDto?> UpdateRouteAsync(int id, UpdateRouteDto dto);
        Task<bool> DeleteRouteAsync(int id);
        Task<bool> RouteExistsAsync(int id);
    }
}