using System;

namespace RoutingService.Domain.Entities
{
    public class Trip
    {
        public int TripId { get; set; }
        public int SheetId { get; set; }
        public TimeSpan ScheduledDeparture { get; set; }
        public TimeSpan? ActualDeparture { get; set; }
        public bool Completed { get; set; }

        // Navigation properties
        public virtual RouteSheet RouteSheet { get; set; } = null!;
    }
}