using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Commands.UpdateDocument
{
    public class UpdateDocumentCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
        public DateTime? IssuedOn { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? Category { get; set; }
        public string? IssuedBy { get; set; }
        public string? DocumentNumber { get; set; }
    }
}