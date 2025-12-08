using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelById
{
    public class GetPersonnelByIdQuery : IQuery<PersonnelDto>
    {
        public string Id { get; set; } = string.Empty;

        public GetPersonnelByIdQuery(string id)
        {
            Id = id;
        }
    }
}