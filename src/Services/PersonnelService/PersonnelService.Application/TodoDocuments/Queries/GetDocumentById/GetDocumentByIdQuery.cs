using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoDocuments.Queries.GetDocumentById
{
    public class GetDocumentByIdQuery : IQuery<DocumentDto>
    {
        public string Id { get; set; } = string.Empty;

        public GetDocumentByIdQuery(string id)
        {
            Id = id;
        }
    }
}