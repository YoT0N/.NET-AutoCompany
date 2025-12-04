using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Commands.UpdateWorkShift
{
    public class UpdateWorkShiftCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
        public DateTime? ShiftDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        // Bus info (optional)
        public string? BusCountryNumber { get; set; }
        public string? BusBrand { get; set; }

        // Route info (optional)
        public string? RouteNumber { get; set; }
        public double? DistanceKm { get; set; }

        public string? Status { get; set; }
    }
}