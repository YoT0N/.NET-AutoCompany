using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByDateRange
{
    public class GetWorkShiftsByDateRangeQuery : IQuery<IReadOnlyCollection<WorkShiftDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? PersonnelId { get; set; }

        public GetWorkShiftsByDateRangeQuery(DateTime startDate, DateTime endDate, int? personnelId = null)
        {
            StartDate = startDate;
            EndDate = endDate;
            PersonnelId = personnelId;
        }
    }
}