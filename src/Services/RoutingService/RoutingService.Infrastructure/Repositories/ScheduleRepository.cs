using Microsoft.EntityFrameworkCore;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Dal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Dal.Repositories
{
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(RoutingDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByTimeRangeAsync(
            TimeSpan startTime,
            TimeSpan endTime)
        {
            return await _dbSet
                .Where(s => s.DepartureTime >= startTime && s.DepartureTime <= endTime)
                .Include(s => s.Route)
                .OrderBy(s => s.DepartureTime)
                .ThenBy(s => s.Route.RouteNumber)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetScheduleCountByRouteAsync()
        {
            return await _dbSet
                .GroupBy(s => s.RouteId)
                .Select(g => new { RouteId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RouteId, x => x.Count);
        }

        public async Task<IEnumerable<RouteStopScheduleInfo>> GetRouteStopsWithSchedulesAsync(int routeId)
        {
            var result = await (
                from route in _context.Routes
                where route.RouteId == routeId
                join assignment in _context.RouteStopAssignments
                    on route.RouteId equals assignment.RouteId
                join stop in _context.RouteStops
                    on assignment.StopId equals stop.StopId
                join schedule in _context.Schedules
                    on route.RouteId equals schedule.RouteId
                select new
                {
                    route.RouteId,
                    route.RouteNumber,
                    stop.StopId,
                    stop.StopName,
                    assignment.StopOrder,
                    schedule.DepartureTime
                })
                .GroupBy(x => new
                {
                    x.RouteId,
                    x.RouteNumber,
                    x.StopId,
                    x.StopName,
                    x.StopOrder
                })
                .Select(g => new RouteStopScheduleInfo
                {
                    RouteId = g.Key.RouteId,
                    RouteNumber = g.Key.RouteNumber,
                    StopId = g.Key.StopId,
                    StopName = g.Key.StopName,
                    StopOrder = g.Key.StopOrder,
                    DepartureTimes = g.Select(x => x.DepartureTime)
                                      .Distinct()
                                      .OrderBy(t => t)
                                      .ToList()
                })
                .OrderBy(x => x.StopOrder)
                .ToListAsync();

            return result;
        }

        public async Task<(TimeSpan? FirstDeparture, TimeSpan? LastDeparture)> GetRouteOperatingHoursAsync(int routeId)
        {
            var schedules = await _dbSet
                .Where(s => s.RouteId == routeId)
                .OrderBy(s => s.DepartureTime)
                .Select(s => s.DepartureTime)
                .ToListAsync();

            if (!schedules.Any())
                return (null, null);

            return (schedules.First(), schedules.Last());
        }
    }
}