using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Commands.DeletePersonnel
{
    public class DeletePersonnelCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
    }
}