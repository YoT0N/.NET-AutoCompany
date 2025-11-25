namespace PersonnelService.Core.DTOs
{
    public class WorkShiftDto
    {
        public string? Id { get; set; }
        public int PersonnelId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public BusInfoDto Bus { get; set; } = new BusInfoDto();
        public RouteInfoDto Route { get; set; } = new RouteInfoDto();
        public string Status { get; set; } = string.Empty;
    }

    public class BusInfoDto
    {
        public string BusCountryNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
    }

    public class RouteInfoDto
    {
        public string RouteNumber { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
    }

    public class CreateWorkShiftDto
    {
        public int PersonnelId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public BusInfoDto Bus { get; set; } = new BusInfoDto();
        public RouteInfoDto Route { get; set; } = new RouteInfoDto();
        public string Status { get; set; } = "Scheduled";
    }

    public class UpdateWorkShiftDto
    {
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public BusInfoDto Bus { get; set; } = new BusInfoDto();
        public RouteInfoDto Route { get; set; } = new RouteInfoDto();
        public string Status { get; set; } = string.Empty;
    }
}