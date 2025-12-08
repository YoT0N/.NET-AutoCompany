using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Dal.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Dal.Repositories
{
    public class RouteRepository : Repository<Route>, IRouteRepository
    {
        public RouteRepository(RoutingDbContext context) : base(context)
        {
        }

        public async Task<Route?> GetRouteWithStopsAsync(int routeId)
        {
            return await _dbSet
                .Include(r => r.RouteStopAssignments.OrderBy(rsa => rsa.StopOrder))
                    .ThenInclude(rsa => rsa.RouteStop)
                .FirstOrDefaultAsync(r => r.RouteId == routeId);
        }

        public async Task<IEnumerable<Route>> GetRoutesWithSchedulesAsync()
        {
            return await _dbSet
                .Include(r => r.Schedules.OrderBy(s => s.DepartureTime))
                .OrderBy(r => r.RouteNumber)
                .ToListAsync();
        }

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