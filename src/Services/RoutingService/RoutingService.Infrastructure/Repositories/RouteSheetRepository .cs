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
    public class RouteSheetRepository : Repository<RouteSheet>, IRouteSheetRepository
    {
        public RouteSheetRepository(RoutingDbContext context) : base(context)
        {
        }

        public async Task<RouteSheet?> GetRouteSheetWithRouteAsync(int sheetId)
        {
            var routeSheet = await _dbSet.FindAsync(sheetId);

            if (routeSheet != null)
            {
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.Route)
                    .LoadAsync();
            }

            return routeSheet;
        }

        public async Task<RouteSheet?> GetRouteSheetWithBusInfoAsync(int sheetId)
        {
            var routeSheet = await _dbSet.FindAsync(sheetId);

            if (routeSheet != null)
            {
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.BusInfo)
                    .LoadAsync();
            }

            return routeSheet;
        }

        public async Task<RouteSheet?> GetRouteSheetWithTripsAsync(int sheetId)
        {
            var routeSheet = await _dbSet.FindAsync(sheetId);

            if (routeSheet != null)
            {
                await _context.Entry(routeSheet)
                    .Collection(rs => rs.Trips)
                    .Query()
                    .OrderBy(t => t.ScheduledDeparture)
                    .LoadAsync();
            }

            return routeSheet;
        }

        public async Task<IEnumerable<RouteSheet>> GetRouteSheetsByDateWithDetailsAsync(DateTime date)
        {
            var routeSheets = await _dbSet
                .Where(rs => rs.SheetDate.Date == date.Date)
                .ToListAsync();

            foreach (var routeSheet in routeSheets)
            {
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.Route)
                    .LoadAsync();

                await _context.Entry(routeSheet)
                    .Reference(rs => rs.BusInfo)
                    .LoadAsync();

                await _context.Entry(routeSheet)
                    .Collection(rs => rs.Trips)
                    .Query()
                    .Where(t => !t.Completed)
                    .OrderBy(t => t.ScheduledDeparture)
                    .LoadAsync();
            }

            return routeSheets;
        }
    }
}