using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoDocuments.Queries.GetDocumentsByPersonnel
{
    public class GetDocumentsByPersonnelQuery : IQuery<IReadOnlyCollection<DocumentDto>>
    {
        public int PersonnelId { get; set; }

        public GetDocumentsByPersonnelQuery(int personnelId)
        {
            PersonnelId = personnelId;
        }
    }
}