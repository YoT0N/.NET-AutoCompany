using FluentValidation;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Commands.CreateWorkShift
{
    public class CreateWorkShiftCommandValidator : AbstractValidator<CreateWorkShiftCommand>
    {
        public CreateWorkShiftCommandValidator(IPersonnelRepository personnelRepository)
        {
            RuleFor(x => x.PersonnelId)
                .GreaterThan(0).WithMessage("PersonnelId must be positive.")
                .MustAsync(async (personnelId, cancellation) =>
                    await personnelRepository.ExistsByPersonnelIdAsync(personnelId, cancellation))
                .WithMessage("Personnel with this ID does not exist.");

            RuleFor(x => x.ShiftDate)
                .NotEmpty().WithMessage("Shift date is required.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.")
                .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("Start time must be in format HH:mm.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("End time must be in format HH:mm.");

            RuleFor(x => x.BusCountryNumber)
                .NotEmpty().WithMessage("Bus country number is required.")
                .MaximumLength(20).WithMessage("Bus country number cannot exceed 20 characters.");

            RuleFor(x => x.BusBrand)
                .NotEmpty().WithMessage("Bus brand is required.")
                .MaximumLength(50).WithMessage("Bus brand cannot exceed 50 characters.");

            RuleFor(x => x.RouteNumber)
                .NotEmpty().WithMessage("Route number is required.")
                .MaximumLength(20).WithMessage("Route number cannot exceed 20 characters.");

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage("Distance must be positive.")
                .LessThanOrEqualTo(500).WithMessage("Distance cannot exceed 500 km.");
        }
    }
}