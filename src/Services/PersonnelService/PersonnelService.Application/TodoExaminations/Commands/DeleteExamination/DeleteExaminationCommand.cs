using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Commands.DeleteExamination
{
    public class DeleteExaminationCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
    }
}