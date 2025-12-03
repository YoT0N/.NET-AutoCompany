using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Commands.CreateExamination
{
    public class CreateExaminationCommand : ICommand<string>
    {
        public int PersonnelId { get; set; }
        public DateTime ExamDate { get; set; }
        public string Result { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Metrics
        public int Height { get; set; }
        public int Weight { get; set; }
        public string BloodPressure { get; set; } = string.Empty;
        public string Vision { get; set; } = string.Empty;
    }
}