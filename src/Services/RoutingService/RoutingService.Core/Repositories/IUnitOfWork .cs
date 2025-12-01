using RoutingService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    /// <summary>
    /// Unit of Work interface
    /// Provides access to all repositories and transaction management
    /// Coordinates multiple repository operations in a single transaction
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Generic repositories for basic CRUD operations
        IRepository<Route> Routes { get; }
        IRepository<RouteStop> RouteStops { get; }
        IRepository<RouteStopAssignment> RouteStopAssignments { get; }
        IRepository<BusInfo> Buses { get; }
        IRepository<RouteSheet> RouteSheets { get; }
        IRepository<Schedule> Schedules { get; }
        IRepository<Trip> Trips { get; }

        // Specific repositories with custom queries
        IRouteRepository RouteRepository { get; }
        IRouteSheetRepository RouteSheetRepository { get; }
        IScheduleRepository ScheduleRepository { get; }

        // Transaction management methods
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}