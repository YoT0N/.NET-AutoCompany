using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftById
{
    public class GetWorkShiftByIdQuery : IQuery<WorkShiftDto>
    {
        public string Id { get; set; } = string.Empty;

        public GetWorkShiftByIdQuery(string id)
        {
            Id = id;
        }
    }
}