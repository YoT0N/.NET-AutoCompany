using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Commands.UpdateExamination
{
    public class UpdateExaminationCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
        public string? Result { get; set; }
        public string? DoctorName { get; set; }
        public string? Notes { get; set; }

        // Metrics (optional updates)
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string? BloodPressure { get; set; }
        public string? Vision { get; set; }
    }
}