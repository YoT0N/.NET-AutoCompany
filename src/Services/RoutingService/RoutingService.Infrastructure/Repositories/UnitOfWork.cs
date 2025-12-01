using System;
using System.Threading.Tasks;
using RoutingService.Core.Entities;
using RoutingService.Domain.Interfaces.Repositories;
using RoutingService.Infrastructure.Data;

namespace RoutingService.Dal.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RoutingDbContext _context;
        private IRepository<Route>? _routes;
        private IRepository<RouteStop>? _routeStops;
        private IRepository<RouteStopAssignment>? _routeStopAssignments;
        private IRepository<BusInfo>? _buses;
        private IRepository<RouteSheet>? _routeSheets;
        private IRepository<Schedule>? _schedules;
        private IRepository<Trip>? _trips;

        public UnitOfWork(RoutingDbContext context)
        {
            _context = context;
        }

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

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}