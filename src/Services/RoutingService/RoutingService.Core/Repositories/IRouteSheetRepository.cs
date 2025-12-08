using RoutingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    public interface IRouteSheetRepository : IRepository<RouteSheet>
    {
        Task<RouteSheet?> GetRouteSheetWithRouteAsync(int sheetId);

        Task<RouteSheet?> GetRouteSheetWithBusInfoAsync(int sheetId);

        Task<RouteSheet?> GetRouteSheetWithTripsAsync(int sheetId);

        Task<IEnumerable<RouteSheet>> GetRouteSheetsByDateWithDetailsAsync(DateTime date);
    }
}