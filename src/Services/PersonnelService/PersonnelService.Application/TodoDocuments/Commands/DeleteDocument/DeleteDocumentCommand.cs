using PersonnelService.Application.Common.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Commands.DeleteDocument
{
    public class DeleteDocumentCommand : ICommand<string>
    {
        public string Id { get; set; } = string.Empty;
    }
}