using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoExaminations.Queries.GetExaminationsByPersonnel
{
    public class GetExaminationsByPersonnelQuery : IQuery<IReadOnlyCollection<ExaminationDto>>
    {
        public int PersonnelId { get; set; }

        public GetExaminationsByPersonnelQuery(int personnelId)
        {
            PersonnelId = personnelId;
        }
    }
}