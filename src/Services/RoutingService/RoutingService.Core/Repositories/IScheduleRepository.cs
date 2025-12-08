using RoutingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutingService.Domain.Repositories
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetSchedulesByTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);

        Task<Dictionary<int, int>> GetScheduleCountByRouteAsync();

        Task<IEnumerable<RouteStopScheduleInfo>> GetRouteStopsWithSchedulesAsync(int routeId);

        Task<(TimeSpan? FirstDeparture, TimeSpan? LastDeparture)> GetRouteOperatingHoursAsync(int routeId);
    }

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