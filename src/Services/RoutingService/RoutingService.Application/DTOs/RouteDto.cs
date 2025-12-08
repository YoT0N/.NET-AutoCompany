namespace RoutingService.Bll.DTOs
{
    public class RouteDto
    {
        public int RouteId { get; set; }
        public string RouteNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateRouteDto
    {
        public string RouteNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
    }

    public class UpdateRouteDto
    {
        public string? RouteNumber { get; set; }
        public string? Name { get; set; }
        public decimal? DistanceKm { get; set; }
    }

    public class RouteWithStopsDto : RouteDto
    {
        public List<RouteStopInfoDto> Stops { get; set; } = new();
    }

    public class RouteStopInfoDto
    {
        public int StopId { get; set; }
        public string StopName { get; set; } = string.Empty;
        public int StopOrder { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}