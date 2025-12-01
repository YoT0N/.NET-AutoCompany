using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Infrastructure.Repositories
{
    /// <summary>
    /// RouteSheet-specific repository with Explicit Loading examples
    /// Demonstrates: Entry(), Reference(), Collection(), Load()
    /// </summary>
    public class RouteSheetRepository : Repository<RouteSheet>, IRouteSheetRepository
    {
        public RouteSheetRepository(RoutingDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Explicit Loading: Load Route navigation property explicitly
        /// First gets the entity, then explicitly loads the navigation property
        /// </summary>
        public async Task<RouteSheet?> GetRouteSheetWithRouteAsync(int sheetId)
        {
            var routeSheet = await _dbSet.FindAsync(sheetId);

            if (routeSheet != null)
            {
                // Explicitly load the Route navigation property
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.Route)
                    .LoadAsync();
            }

            return routeSheet;
        }

        /// <summary>
        /// Explicit Loading: Load BusInfo navigation property explicitly
        /// </summary>
        public async Task<RouteSheet?> GetRouteSheetWithBusInfoAsync(int sheetId)
        {
            var routeSheet = await _dbSet.FindAsync(sheetId);

            if (routeSheet != null)
            {
                // Explicitly load the BusInfo navigation property
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.BusInfo)
                    .LoadAsync();
            }

            return routeSheet;
        }

        /// <summary>
        /// Explicit Loading: Load collection navigation property (Trips)
        /// Demonstrates loading a collection with filtering and ordering
        /// </summary>
        public async Task<RouteSheet?> GetRouteSheetWithTripsAsync(int sheetId)
        {
            var routeSheet = await _dbSet.FindAsync(sheetId);

            if (routeSheet != null)
            {
                // Explicitly load the Trips collection
                // Can also apply query operations
                await _context.Entry(routeSheet)
                    .Collection(rs => rs.Trips)
                    .Query()
                    .OrderBy(t => t.ScheduledDeparture)
                    .LoadAsync();
            }

            return routeSheet;
        }

        /// <summary>
        /// Explicit Loading: Load multiple navigation properties step-by-step
        /// Demonstrates conditional loading based on what data is needed
        /// </summary>
        public async Task<IEnumerable<RouteSheet>> GetRouteSheetsByDateWithDetailsAsync(DateTime date)
        {
            // First, get the basic route sheets for the date
            var routeSheets = await _dbSet
                .Where(rs => rs.SheetDate.Date == date.Date)
                .ToListAsync();

            // Then explicitly load related data for each route sheet
            foreach (var routeSheet in routeSheets)
            {
                // Load Route
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.Route)
                    .LoadAsync();

                // Load BusInfo
                await _context.Entry(routeSheet)
                    .Reference(rs => rs.BusInfo)
                    .LoadAsync();

                // Load Trips (only non-completed trips)
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