using Microsoft.EntityFrameworkCore.Storage;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace RoutingService.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work pattern implementation
    /// Manages transactions and coordinates repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RoutingDbContext _context;

        // Generic repositories
        private IRepository<Route>? _routes;
        private IRepository<RouteStop>? _routeStops;
        private IRepository<RouteStopAssignment>? _routeStopAssignments;
        private IRepository<BusInfo>? _buses;
        private IRepository<RouteSheet>? _routeSheets;
        private IRepository<Schedule>? _schedules;
        private IRepository<Trip>? _trips;

        // Specific repositories
        private IRouteRepository? _routeRepository;
        private IRouteSheetRepository? _routeSheetRepository;
        private IScheduleRepository? _scheduleRepository;

        public UnitOfWork(RoutingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Generic repository properties
        public IRepository<Route> Routes
        {
            get { return _routes ??= new Repository<Route>(_context); }
        }

        public IRepository<RouteStop> RouteStops
        {
            get { return _routeStops ??= new Repository<RouteStop>(_context); }
        }

        public IRepository<RouteStopAssignment> RouteStopAssignments
        {
            get { return _routeStopAssignments ??= new Repository<RouteStopAssignment>(_context); }
        }

        public IRepository<BusInfo> Buses
        {
            get { return _buses ??= new Repository<BusInfo>(_context); }
        }

        public IRepository<RouteSheet> RouteSheets
        {
            get { return _routeSheets ??= new Repository<RouteSheet>(_context); }
        }

        public IRepository<Schedule> Schedules
        {
            get { return _schedules ??= new Repository<Schedule>(_context); }
        }

        public IRepository<Trip> Trips
        {
            get { return _trips ??= new Repository<Trip>(_context); }
        }

        // Specific repository properties
        public IRouteRepository RouteRepository
        {
            get { return _routeRepository ??= new RouteRepository(_context); }
        }

        public IRouteSheetRepository RouteSheetRepository
        {
            get { return _routeSheetRepository ??= new RouteSheetRepository(_context); }
        }

        public IScheduleRepository ScheduleRepository
        {
            get { return _scheduleRepository ??= new ScheduleRepository(_context); }
        }

        /// <summary>
        /// Save all changes to the database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Begin a database transaction
        /// </summary>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}