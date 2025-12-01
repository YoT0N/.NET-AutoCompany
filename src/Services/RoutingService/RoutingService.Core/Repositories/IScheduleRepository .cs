using RoutingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    /// <summary>
    /// Schedule-specific repository interface
    /// Demonstrates LINQ to Entities and Many-to-Many queries
    /// </summary>
    public interface IScheduleRepository : IRepository<Schedule>
    {
        /// <summary>
        /// LINQ to Entities: Filter schedules by time range
        /// </summary>
        Task<IEnumerable<Schedule>> GetSchedulesByTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);

        /// <summary>
        /// LINQ to Entities: GroupBy and aggregate operations
        /// </summary>
        Task<Dictionary<int, int>> GetScheduleCountByRouteAsync();

        /// <summary>
        /// LINQ to Entities: Many-to-Many through intermediate entity (RouteStopAssignment)
        /// Complex query demonstrating joins and grouping
        /// </summary>
        Task<IEnumerable<RouteStopScheduleInfo>> GetRouteStopsWithSchedulesAsync(int routeId);
    }

    /// <summary>
    /// DTO for many-to-many query result
    /// Used to demonstrate complex LINQ queries with intermediate entities
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
}