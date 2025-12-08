namespace RoutingService.Domain.Entities
{
    public class RouteStop
    {
        public int StopId { get; set; }
        public string StopName { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Navigation properties
        public virtual ICollection<RouteStopAssignment> RouteStopAssignments { get; set; } = new List<RouteStopAssignment>();
    }
}