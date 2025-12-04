using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoWorkShifts.Commands.UpdateWorkShift
{
    public class UpdateWorkShiftCommandValidator : AbstractValidator<UpdateWorkShiftCommand>
    {
        public UpdateWorkShiftCommandValidator(IWorkShiftRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Work shift Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Work shift not found.");

            When(x => !string.IsNullOrWhiteSpace(x.StartTime), () =>
            {
                RuleFor(x => x.StartTime)
                    .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
                    .WithMessage("Start time must be in format HH:mm.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.EndTime), () =>
            {
                RuleFor(x => x.EndTime)
                    .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
                    .WithMessage("End time must be in format HH:mm.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.BusCountryNumber), () =>
            {
                RuleFor(x => x.BusCountryNumber)
                    .MaximumLength(20).WithMessage("Bus country number cannot exceed 20 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.BusBrand), () =>
            {
                RuleFor(x => x.BusBrand)
                    .MaximumLength(50).WithMessage("Bus brand cannot exceed 50 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.RouteNumber), () =>
            {
                RuleFor(x => x.RouteNumber)
                    .MaximumLength(20).WithMessage("Route number cannot exceed 20 characters.");
            });

            When(x => x.DistanceKm.HasValue, () =>
            {
                RuleFor(x => x.DistanceKm!.Value)
                    .GreaterThan(0).WithMessage("Distance must be positive.")
                    .LessThanOrEqualTo(500).WithMessage("Distance cannot exceed 500 km.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Status), () =>
            {
                RuleFor(x => x.Status)
                    .Must(status => new[] { "Scheduled", "InProgress", "Completed", "Cancelled" }.Contains(status!))
                    .WithMessage("Status must be one of: Scheduled, InProgress, Completed, Cancelled.");
            });
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}