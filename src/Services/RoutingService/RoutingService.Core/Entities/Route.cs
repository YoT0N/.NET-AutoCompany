using System;
using System.Collections.Generic;

namespace RoutingService.Domain.Entities
{
    public class Route
    {
        public int RouteId { get; set; }
        public string RouteNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<RouteStopAssignment> RouteStopAssignments { get; set; } = new List<RouteStopAssignment>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public virtual ICollection<RouteSheet> RouteSheets { get; set; } = new List<RouteSheet>();
    }
}