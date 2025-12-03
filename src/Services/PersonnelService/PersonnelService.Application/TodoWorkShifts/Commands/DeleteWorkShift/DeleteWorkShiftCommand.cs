using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Commands.DeleteWorkShift
{
    public class DeleteWorkShiftCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
    }
}