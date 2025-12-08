namespace RoutingService.Bll.DTOs
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int RouteId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
    }

    public class CreateScheduleDto
    {
        public int RouteId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
    }

    public class UpdateScheduleDto
    {
        public TimeSpan? DepartureTime { get; set; }
        public TimeSpan? ArrivalTime { get; set; }
    }

    public class ScheduleWithRouteDto : ScheduleDto
    {
        public string RouteNumber { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
    }
}