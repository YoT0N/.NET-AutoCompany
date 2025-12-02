using RoutingService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    public interface IRouteRepository : IRepository<Route>
    {
        Task<Route?> GetRouteWithStopsAsync(int routeId);

        Task<IEnumerable<Route>> GetRoutesWithSchedulesAsync();

        Task<Route?> GetRouteWithFullDetailsAsync(int routeId);
    }
}