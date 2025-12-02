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
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        public TripRepository(RoutingDbContext context) : base(context)
        {
        }

        public async Task<Trip?> GetTripWithFullDetailsAsync(int tripId)
        {
            return await _dbSet
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .FirstOrDefaultAsync(t => t.TripId == tripId);
        }

        public async Task<IEnumerable<Trip>> GetTripsByRouteSheetAsync(int sheetId)
        {
            return await _dbSet
                .Where(t => t.SheetId == sheetId)
                .OrderBy(t => t.ScheduledDeparture)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> GetDelayedTripsAsync()
        {
            return await _dbSet
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.BusInfo)
                .Where(t => t.ActualDeparture != null && t.ActualDeparture > t.ScheduledDeparture)
                .OrderByDescending(t => t.ActualDeparture!.Value - t.ScheduledDeparture)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> GetTripsByStatusAndDateAsync(bool completed, DateTime date)
        {
            return await _dbSet
                .Include(t => t.RouteSheet)
                    .ThenInclude(rs => rs.Route)
                .Where(t => t.Completed == completed && t.RouteSheet.SheetDate.Date == date.Date)
                .OrderBy(t => t.ScheduledDeparture)
                .ToListAsync();
        }

        public async Task<TimeSpan?> GetAverageDelayByRouteAsync(int routeId)
        {
            var delays = await _dbSet
                .Include(t => t.RouteSheet)
                .Where(t => t.RouteSheet.RouteId == routeId &&
                           t.ActualDeparture != null &&
                           t.ActualDeparture > t.ScheduledDeparture)
                .Select(t => (t.ActualDeparture!.Value - t.ScheduledDeparture).TotalMinutes)
                .ToListAsync();

            if (!delays.Any())
                return null;

            var avgMinutes = delays.Average();
            return TimeSpan.FromMinutes(avgMinutes);
        }
    }
}