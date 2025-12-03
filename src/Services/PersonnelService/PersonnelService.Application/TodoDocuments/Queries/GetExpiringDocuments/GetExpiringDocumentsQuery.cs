using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoDocuments.Queries.GetExpiringDocuments
{
    public class GetExpiringDocumentsQuery : IQuery<IReadOnlyCollection<DocumentDto>>
    {
        public int DaysThreshold { get; set; } = 30;

        public GetExpiringDocumentsQuery(int daysThreshold = 30)
        {
            DaysThreshold = daysThreshold;
        }
    }
}