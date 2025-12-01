namespace RoutingService.Core.DTOs
{
    public class RouteStopDto
    {
        public int StopId { get; set; }
        public string StopName { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class CreateRouteStopDto
    {
        public string StopName { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class UpdateRouteStopDto
    {
        public string? StopName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class AssignStopToRouteDto
    {
        public int RouteId { get; set; }
        public int StopId { get; set; }
        public int StopOrder { get; set; }
    }
}