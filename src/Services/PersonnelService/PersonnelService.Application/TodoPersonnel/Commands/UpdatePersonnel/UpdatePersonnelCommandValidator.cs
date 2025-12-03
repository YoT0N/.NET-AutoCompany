using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnel
{
    public class UpdatePersonnelCommandValidator : AbstractValidator<UpdatePersonnelCommand>
    {
        public UpdatePersonnelCommandValidator(IPersonnelRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Personnel Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Personnel not found.");

            When(x => !string.IsNullOrWhiteSpace(x.FullName), () =>
            {
                RuleFor(x => x.FullName)
                    .MinimumLength(3).WithMessage("Full name must be at least 3 characters.");
            });

            When(x => x.BirthDate.HasValue, () =>
            {
                RuleFor(x => x.BirthDate!.Value)
                    .Must(date => date <= DateTime.UtcNow.AddYears(-18))
                    .WithMessage("Personnel must be at least 18 years old.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Invalid email format.");
            });
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}