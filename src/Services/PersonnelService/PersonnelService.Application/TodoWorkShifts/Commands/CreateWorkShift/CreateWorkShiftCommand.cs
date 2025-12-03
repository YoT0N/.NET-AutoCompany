using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Commands.CreateWorkShift
{
    public class CreateWorkShiftCommand : ICommand<string>
    {
        public int PersonnelId { get; set; }
        public DateTime ShiftDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;

        // Bus info
        public string BusCountryNumber { get; set; } = string.Empty;
        public string BusBrand { get; set; } = string.Empty;

        // Route info
        public string RouteNumber { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
    }
}