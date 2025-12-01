using RoutingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    /// <summary>
    /// RouteSheet-specific repository interface
    /// Demonstrates Explicit Loading patterns
    /// </summary>
    public interface IRouteSheetRepository : IRepository<RouteSheet>
    {
        /// <summary>
        /// Get route sheet with explicit loading of Route navigation property
        /// </summary>
        Task<RouteSheet?> GetRouteSheetWithRouteAsync(int sheetId);

        /// <summary>
        /// Get route sheet with explicit loading of BusInfo navigation property
        /// </summary>
        Task<RouteSheet?> GetRouteSheetWithBusInfoAsync(int sheetId);

        /// <summary>
        /// Get route sheet with explicit loading of all Trips
        /// </summary>
        Task<RouteSheet?> GetRouteSheetWithTripsAsync(int sheetId);

        /// <summary>
        /// Get route sheets by date with all related data (Explicit Loading example)
        /// </summary>
        Task<IEnumerable<RouteSheet>> GetRouteSheetsByDateWithDetailsAsync(DateTime date);
    }
}