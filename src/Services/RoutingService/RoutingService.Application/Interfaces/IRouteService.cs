using RoutingService.Bll.DTOs.Common;
using RoutingService.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Application.Interfaces
{
    /// <summary>
    /// Route service interface
    /// Handles business logic for route operations
    /// </summary>
    public interface IRouteService
    {
        // Basic CRUD operations
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto> GetRouteByIdAsync(int id);
        Task<RouteWithStopsDto> GetRouteWithStopsAsync(int id);
        Task<RouteDto> CreateRouteAsync(CreateRouteDto dto);
        Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto dto);
        Task DeleteRouteAsync(int id);
        Task<bool> RouteExistsAsync(int id);

        // Pagination, filtering, sorting
        Task<PagedResultDto<RouteDto>> GetRoutesPagedAsync(RouteFilterParameters parameters);
    }
}