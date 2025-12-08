using RoutingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{

    public interface ITripRepository : IRepository<Trip>
    {
        Task<Trip?> GetTripWithFullDetailsAsync(int tripId);

        Task<IEnumerable<Trip>> GetTripsByRouteSheetAsync(int sheetId);

        Task<IEnumerable<Trip>> GetDelayedTripsAsync();

        Task<IEnumerable<Trip>> GetTripsByStatusAndDateAsync(bool completed, DateTime date);

        Task<TimeSpan?> GetAverageDelayByRouteAsync(int routeId);
    }
}