using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByPersonnel
{
    public class GetWorkShiftsByPersonnelQuery : IQuery<IReadOnlyCollection<WorkShiftDto>>
    {
        public int PersonnelId { get; set; }

        public GetWorkShiftsByPersonnelQuery(int personnelId)
        {
            PersonnelId = personnelId;
        }
    }
}