using PersonnelService.Application.Common.Interfaces;
using PersonnelService.Application.Common.Mappings;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetAllPersonnel
{
    public class GetAllPersonnelQuery : IQuery<IReadOnlyCollection<PersonnelDto>>
    {
        public string? SearchText { get; set; }
        public string? Position { get; set; }
        public string? Status { get; set; }
        public int Skip { get; set; } = 0;
        public int Limit { get; set; } = 10;
    }
}