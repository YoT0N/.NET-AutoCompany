using System;
using System.Threading.Tasks;
using RoutingService.Core.Entities;

namespace RoutingService.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Route> Routes { get; }
        IRepository<RouteStop> RouteStops { get; }
        IRepository<RouteStopAssignment> RouteStopAssignments { get; }
        IRepository<BusInfo> Buses { get; }
        IRepository<RouteSheet> RouteSheets { get; }
        IRepository<Schedule> Schedules { get; }
        IRepository<Trip> Trips { get; }

        Task<int> SaveChangesAsync();
    }
}