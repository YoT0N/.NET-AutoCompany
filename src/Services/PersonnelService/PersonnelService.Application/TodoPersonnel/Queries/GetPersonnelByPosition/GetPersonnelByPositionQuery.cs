using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelByPosition
{
    public class GetPersonnelByPositionQuery : IQuery<IReadOnlyCollection<PersonnelDto>>
    {
        public string Position { get; set; } = string.Empty;

        public GetPersonnelByPositionQuery(string position)
        {
            Position = position;
        }
    }
}