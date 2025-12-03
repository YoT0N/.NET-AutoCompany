using FluentValidation;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel
{
    public class CreatePersonnelCommandValidator : AbstractValidator<CreatePersonnelCommand>
    {
        public CreatePersonnelCommandValidator(IPersonnelRepository repository)
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MinimumLength(3).WithMessage("Full name must be at least 3 characters.");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.")
                .Must(date => date <= DateTime.UtcNow.AddYears(-18))
                .WithMessage("Personnel must be at least 18 years old.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone format.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");
        }
    }
}