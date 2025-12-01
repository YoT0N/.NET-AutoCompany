using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Infrastructure.Repositories
{
    /// <summary>
    /// Route-specific repository with Eager Loading examples
    /// Demonstrates: Include(), ThenInclude()
    /// </summary>
    public interface IRouteRepository : IRepository<Route>
    {
        Task<Route?> GetRouteWithStopsAsync(int routeId);
        Task<IEnumerable<Route>> GetRoutesWithSchedulesAsync();
        Task<Route?> GetRouteWithFullDetailsAsync(int routeId);
    }

    public class RouteRepository : Repository<Route>, IRouteRepository
    {
        public RouteRepository(RoutingDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Eager Loading: Get route with all stops in correct order
        /// Uses Include() and ThenInclude() for nested navigation properties
        /// </summary>
        public async Task<Route?> GetRouteWithStopsAsync(int routeId)
        {
            return await _dbSet
                .Include(r => r.RouteStopAssignments
                    .OrderBy(rsa => rsa.StopOrder))
                    .ThenInclude(rsa => rsa.RouteStop)
                .FirstOrDefaultAsync(r => r.RouteId == routeId);
        }

        /// <summary>
        /// Eager Loading: Get all routes with their schedules
        /// Demonstrates loading related collection
        /// </summary>
        public async Task<IEnumerable<Route>> GetRoutesWithSchedulesAsync()
        {
            return await _dbSet
                .Include(r => r.Schedules.OrderBy(s => s.DepartureTime))
                .ToListAsync();
        }

        /// <summary>
        /// Eager Loading: Complex query with multiple includes
        /// Loads route with stops, schedules, and route sheets
        /// </summary>
        public async Task<Route?> GetRouteWithFullDetailsAsync(int routeId)
        {
            return await _dbSet
                .Include(r => r.RouteStopAssignments.OrderBy(rsa => rsa.StopOrder))
                    .ThenInclude(rsa => rsa.RouteStop)
                .Include(r => r.Schedules.OrderBy(s => s.DepartureTime))
                .Include(r => r.RouteSheets)
                    .ThenInclude(rs => rs.BusInfo)
                .AsSplitQuery() // Use split queries for better performance
                .FirstOrDefaultAsync(r => r.RouteId == routeId);
        }
    }
}