using Microsoft.EntityFrameworkCore.Storage;
using RoutingService.Core.Entities;
using RoutingService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace RoutingService.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Unit of Work interface
    /// Provides access to all repositories and transaction management
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Generic repositories
        IRepository<Route> Routes { get; }
        IRepository<RouteStop> RouteStops { get; }
        IRepository<RouteStopAssignment> RouteStopAssignments { get; }
        IRepository<BusInfo> Buses { get; }
        IRepository<RouteSheet> RouteSheets { get; }
        IRepository<Schedule> Schedules { get; }
        IRepository<Trip> Trips { get; }

        // Specific repositories with custom methods
        IRouteRepository RouteRepository { get; }
        IRouteSheetRepository RouteSheetRepository { get; }
        IScheduleRepository ScheduleRepository { get; }

        // Transaction management
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}