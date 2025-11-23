using System;
using System.Collections.Generic;

namespace RoutingService.Core.Entities
{
    public class RouteSheet
    {
        public int SheetId { get; set; }
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public DateTime SheetDate { get; set; }

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual BusInfo BusInfo { get; set; } = null!;
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}