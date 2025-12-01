using RoutingService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    /// <summary>
    /// Route-specific repository interface with custom queries
    /// Demonstrates Eager Loading and complex navigation
    /// </summary>
    public interface IRouteRepository : IRepository<Route>
    {
        /// <summary>
        /// Get route with all stops (Eager Loading)
        /// </summary>
        Task<Route?> GetRouteWithStopsAsync(int routeId);

        /// <summary>
        /// Get all routes with schedules (Eager Loading)
        /// </summary>
        Task<IEnumerable<Route>> GetRoutesWithSchedulesAsync();

        /// <summary>
        /// Get route with full details - stops, schedules, route sheets (Complex Eager Loading)
        /// </summary>
        Task<Route?> GetRouteWithFullDetailsAsync(int routeId);
    }
}