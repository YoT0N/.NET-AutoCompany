using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelById
{
    public class GetPersonnelByIdQueryValidator : AbstractValidator<GetPersonnelByIdQuery>
    {
        public GetPersonnelByIdQueryValidator(IPersonnelRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Personnel Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.");
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}