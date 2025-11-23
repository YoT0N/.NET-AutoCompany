using System;

namespace RoutingService.Core.DTOs
{
    public class TripDto
    {
        public int TripId { get; set; }
        public int SheetId { get; set; }
        public TimeSpan ScheduledDeparture { get; set; }
        public TimeSpan? ActualDeparture { get; set; }
        public bool Completed { get; set; }
    }

    public class CreateTripDto
    {
        public int SheetId { get; set; }
        public TimeSpan ScheduledDeparture { get; set; }
    }

    public class UpdateTripDto
    {
        public TimeSpan? ScheduledDeparture { get; set; }
        public TimeSpan? ActualDeparture { get; set; }
        public bool? Completed { get; set; }
    }

    public class TripDetailsDto : TripDto
    {
        public DateTime SheetDate { get; set; }
        public string RouteNumber { get; set; } = string.Empty;
        public string BusCountryNumber { get; set; } = string.Empty;
    }
}