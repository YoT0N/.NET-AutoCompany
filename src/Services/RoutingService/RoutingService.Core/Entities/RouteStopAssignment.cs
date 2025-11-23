namespace RoutingService.Core.Entities
{
    public class RouteStopAssignment
    {
        public int RouteId { get; set; }
        public int StopId { get; set; }
        public int StopOrder { get; set; }

        // Navigation properties
        public virtual Route Route { get; set; } = null!;
        public virtual RouteStop RouteStop { get; set; } = null!;
    }
}