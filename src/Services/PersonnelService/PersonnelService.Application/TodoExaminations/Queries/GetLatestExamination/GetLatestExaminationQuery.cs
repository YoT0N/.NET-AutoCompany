using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoExaminations.Queries.GetLatestExamination
{
    public class GetLatestExaminationQuery : IQuery<ExaminationDto?>
    {
        public int PersonnelId { get; set; }

        public GetLatestExaminationQuery(int personnelId)
        {
            PersonnelId = personnelId;
        }
    }
}