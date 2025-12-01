using Microsoft.EntityFrameworkCore;
using RoutingService.Core.Entities;
using RoutingService.Domain.Entities;
using RoutingService.Domain.Repositories;
using RoutingService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Infrastructure.Repositories
{
    /// <summary>
    /// Schedule-specific repository with LINQ to Entities examples
    /// Demonstrates: Complex LINQ queries, GroupBy, Join, Many-to-Many through intermediate entity
    /// </summary>
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetSchedulesByTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);
        Task<Dictionary<int, int>> GetScheduleCountByRouteAsync();
        Task<IEnumerable<RouteStopScheduleInfo>> GetRouteStopsWithSchedulesAsync(int routeId);
    }

    /// <summary>
    /// DTO for many-to-many query result
    /// </summary>
    public class RouteStopScheduleInfo
    {
        public int RouteId { get; set; }
        public string RouteNumber { get; set; } = string.Empty;
        public int StopId { get; set; }
        public string StopName { get; set; } = string.Empty;
        public int StopOrder { get; set; }
        public List<TimeSpan> DepartureTimes { get; set; } = new();
    }

    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(RoutingDbContext context) : base(context)
        {
        }

        /// <summary>
        /// LINQ to Entities: Filter schedules by time range
        /// Demonstrates: Where, OrderBy, complex predicates
        /// </summary>
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

        /// <summary>
        /// LINQ to Entities: GroupBy and aggregate
        /// Demonstrates: GroupBy, Count, ToDictionary
        /// </summary>
        public async Task<Dictionary<int, int>> GetScheduleCountByRouteAsync()
        {
            return await _dbSet
                .GroupBy(s => s.RouteId)
                .Select(g => new { RouteId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RouteId, x => x.Count);
        }

        /// <summary>
        /// LINQ to Entities: Many-to-Many through intermediate entity (RouteStopAssignment)
        /// Demonstrates: Complex join, SelectMany, GroupBy with multiple properties
        /// 
        /// This query:
        /// 1. Gets route stops for a specific route (through RouteStopAssignment)
        /// 2. Joins with schedules for that route
        /// 3. Groups by stop to show all departure times for each stop
        /// </summary>
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
    }
}