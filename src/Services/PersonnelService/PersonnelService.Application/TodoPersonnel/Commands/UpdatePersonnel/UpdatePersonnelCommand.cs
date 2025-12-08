using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnel
{
    public class UpdatePersonnelCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}