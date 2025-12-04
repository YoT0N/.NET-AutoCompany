using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoExaminations.Queries.GetExaminationById
{
    public class GetExaminationByIdQuery : IQuery<ExaminationDto>
    {
        public string Id { get; set; } = string.Empty;

        public GetExaminationByIdQuery(string id)
        {
            Id = id;
        }
    }
}