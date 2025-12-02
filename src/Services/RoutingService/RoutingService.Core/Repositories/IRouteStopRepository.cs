using RoutingService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    public interface IRouteStopRepository : IRepository<RouteStop>
    {
        Task<RouteStop?> GetStopWithRoutesAsync(int stopId);

        Task<IEnumerable<RouteStop>> SearchStopsByNameAsync(string searchTerm);

        Task<IEnumerable<RouteStop>> GetStopsInAreaAsync(
            decimal minLat, decimal maxLat,
            decimal minLon, decimal maxLon);

        Task<IEnumerable<RouteStop>> GetUnassignedStopsAsync();
    }
}