using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel
{
    public class CreatePersonnelCommand : ICommand<string>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}