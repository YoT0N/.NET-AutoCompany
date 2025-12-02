using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Dal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Dal.Repositories
{
    public class BusInfoRepository : Repository<BusInfo>, IBusInfoRepository
    {
        public BusInfoRepository(RoutingDbContext context) : base(context)
        {
        }

        public async Task<BusInfo?> GetBusWithRouteSheetsAsync(int busId)
        {
            return await _dbSet
                .Include(b => b.RouteSheets)
                    .ThenInclude(rs => rs.Route)
                .FirstOrDefaultAsync(b => b.BusId == busId);
        }

        public async Task<IEnumerable<BusInfo>> GetBusesByBrandAsync(string brand, int? minCapacity = null)
        {
            var query = _dbSet.Where(b => b.Brand != null && b.Brand.ToLower().Contains(brand.ToLower()));

            if (minCapacity.HasValue)
            {
                query = query.Where(b => b.Capacity >= minCapacity.Value);
            }

            return await query
                .OrderBy(b => b.CountryNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<BusInfo>> GetBusesByYearRangeAsync(int startYear, int endYear)
        {
            return await _dbSet
                .Where(b => b.YearOfManufacture >= startYear && b.YearOfManufacture <= endYear)
                .OrderByDescending(b => b.YearOfManufacture)
                .ToListAsync();
        }

        public async Task<IEnumerable<BusInfo>> GetAvailableBusesAsync(DateTime date)
        {
            var assignedBusIds = await _context.RouteSheets
                .Where(rs => rs.SheetDate.Date == date.Date)
                .Select(rs => rs.BusId)
                .Distinct()
                .ToListAsync();

            return await _dbSet
                .Where(b => !assignedBusIds.Contains(b.BusId))
                .OrderBy(b => b.CountryNumber)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetBusTripStatisticsAsync()
        {
            return await _context.RouteSheets
                .GroupBy(rs => rs.BusId)
                .Select(g => new { BusId = g.Key, TripCount = g.Sum(rs => rs.Trips.Count) })
                .ToDictionaryAsync(x => x.BusId, x => x.TripCount);
        }
    }
}