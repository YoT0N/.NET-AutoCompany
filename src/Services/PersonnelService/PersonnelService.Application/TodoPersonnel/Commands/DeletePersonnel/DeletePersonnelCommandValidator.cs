using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoPersonnel.Commands.DeletePersonnel
{
    public class DeletePersonnelCommandValidator : AbstractValidator<DeletePersonnelCommand>
    {
        public DeletePersonnelCommandValidator(IPersonnelRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Personnel Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Personnel not found.");
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}