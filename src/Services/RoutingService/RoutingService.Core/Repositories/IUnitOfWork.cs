using RoutingService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
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

        IRouteRepository RouteRepository { get; }
        IRouteSheetRepository RouteSheetRepository { get; }
        IScheduleRepository ScheduleRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}