using Microsoft.EntityFrameworkCore.Storage;
using RoutingService.Dal.Data;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace RoutingService.Dal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RoutingDbContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<Route>? _routes;
        private IRepository<RouteStop>? _routeStops;
        private IRepository<RouteStopAssignment>? _routeStopAssignments;
        private IRepository<BusInfo>? _buses;
        private IRepository<RouteSheet>? _routeSheets;
        private IRepository<Schedule>? _schedules;
        private IRepository<Trip>? _trips;

        private IRouteRepository? _routeRepository;
        private IRouteSheetRepository? _routeSheetRepository;
        private IScheduleRepository? _scheduleRepository;
        private IBusInfoRepository? _busInfoRepository;
        private IRouteStopRepository? _routeStopRepository;
        private ITripRepository? _tripRepository;

        public UnitOfWork(RoutingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<Route> Routes
            => _routes ??= new Repository<Route>(_context);

        public IRepository<RouteStop> RouteStops
            => _routeStops ??= new Repository<RouteStop>(_context);

        public IRepository<RouteStopAssignment> RouteStopAssignments
            => _routeStopAssignments ??= new Repository<RouteStopAssignment>(_context);

        public IRepository<BusInfo> Buses
            => _buses ??= new Repository<BusInfo>(_context);

        public IRepository<RouteSheet> RouteSheets
            => _routeSheets ??= new Repository<RouteSheet>(_context);

        public IRepository<Schedule> Schedules
            => _schedules ??= new Repository<Schedule>(_context);

        public IRepository<Trip> Trips
            => _trips ??= new Repository<Trip>(_context);

        // Specific repository properties
        public IRouteRepository RouteRepository
            => _routeRepository ??= new RouteRepository(_context);

        public IRouteSheetRepository RouteSheetRepository
            => _routeSheetRepository ??= new RouteSheetRepository(_context);

        public IScheduleRepository ScheduleRepository
            => _scheduleRepository ??= new ScheduleRepository(_context);

        public IBusInfoRepository BusInfoRepository
            => _busInfoRepository ??= new BusInfoRepository(_context);

        public IRouteStopRepository RouteStopRepository
            => _routeStopRepository ??= new RouteStopRepository(_context);

        public ITripRepository TripRepository
            => _tripRepository ??= new TripRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction not started");

            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}