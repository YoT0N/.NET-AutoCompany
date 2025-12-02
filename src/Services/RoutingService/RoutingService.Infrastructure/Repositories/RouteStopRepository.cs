using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Dal.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Dal.Repositories
{
    public class RouteStopRepository : Repository<RouteStop>, IRouteStopRepository
    {
        public RouteStopRepository(RoutingDbContext context) : base(context)
        {
        }

        public async Task<RouteStop?> GetStopWithRoutesAsync(int stopId)
        {
            return await _dbSet
                .Include(rs => rs.RouteStopAssignments)
                    .ThenInclude(rsa => rsa.Route)
                .FirstOrDefaultAsync(rs => rs.StopId == stopId);
        }

        public async Task<IEnumerable<RouteStop>> SearchStopsByNameAsync(string searchTerm)
        {
            return await _dbSet
                .Where(rs => rs.StopName.ToLower().Contains(searchTerm.ToLower()))
                .OrderBy(rs => rs.StopName)
                .ToListAsync();
        }

        public async Task<IEnumerable<RouteStop>> GetStopsInAreaAsync(
            decimal minLat, decimal maxLat,
            decimal minLon, decimal maxLon)
        {
            return await _dbSet
                .Where(rs => rs.Latitude != null && rs.Longitude != null &&
                            rs.Latitude >= minLat && rs.Latitude <= maxLat &&
                            rs.Longitude >= minLon && rs.Longitude <= maxLon)
                .OrderBy(rs => rs.StopName)
                .ToListAsync();
        }

        public async Task<IEnumerable<RouteStop>> GetUnassignedStopsAsync()
        {
            return await _dbSet
                .Where(rs => !rs.RouteStopAssignments.Any())
                .OrderBy(rs => rs.StopName)
                .ToListAsync();
        }
    }
}