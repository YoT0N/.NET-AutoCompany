using System;

namespace RoutingService.Domain.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int RouteId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
    }
}