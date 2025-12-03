using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnelStatus
{
    public class UpdatePersonnelStatusCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}