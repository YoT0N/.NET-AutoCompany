using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Commands.CreateDocument
{
    public class CreateDocumentCommand : ICommand<string>
    {
        public int PersonnelId { get; set; }
        public string DocType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public DateTime? IssuedOn { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? Category { get; set; }
        public string? IssuedBy { get; set; }
        public string? DocumentNumber { get; set; }
    }
}