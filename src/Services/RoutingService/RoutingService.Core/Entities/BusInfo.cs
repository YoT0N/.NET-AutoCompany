using System.Collections.Generic;

namespace RoutingService.Domain.Entities
{
    public class BusInfo
    {
        public int BusId { get; set; }
        public string CountryNumber { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public int? Capacity { get; set; }
        public int? YearOfManufacture { get; set; }

        // Navigation properties
        public virtual ICollection<RouteSheet> RouteSheets { get; set; } = new List<RouteSheet>();
    }
}