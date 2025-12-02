using FluentValidation;
using RoutingService.Bll.DTOs;
using System;

namespace RoutingService.Bll.Validators.TripValidators
{
    public class CreateTripDtoValidator : AbstractValidator<CreateTripDto>
    {
        public CreateTripDtoValidator()
        {
            RuleFor(x => x.SheetId)
                .GreaterThan(0).WithMessage("Sheet ID must be greater than 0");

            RuleFor(x => x.ScheduledDeparture)
                .NotEmpty().WithMessage("Scheduled departure is required")
                .Must(BeValidTime).WithMessage("Scheduled departure must be between 00:00 and 23:59");
        }

        private bool BeValidTime(TimeSpan time)
        {
            return time >= TimeSpan.Zero && time < TimeSpan.FromHours(24);
        }
    }

    public class UpdateTripDtoValidator : AbstractValidator<UpdateTripDto>
    {
        public UpdateTripDtoValidator()
        {
            When(x => x.ScheduledDeparture.HasValue, () =>
            {
                RuleFor(x => x.ScheduledDeparture)
                    .Must(time => BeValidTime(time!.Value))
                    .WithMessage("Scheduled departure must be between 00:00 and 23:59");
            });

            When(x => x.ActualDeparture.HasValue, () =>
            {
                RuleFor(x => x.ActualDeparture)
                    .Must(time => BeValidTime(time!.Value))
                    .WithMessage("Actual departure must be between 00:00 and 23:59");
            });
        }

        private bool BeValidTime(TimeSpan time)
        {
            return time >= TimeSpan.Zero && time < TimeSpan.FromHours(24);
        }
    }
}