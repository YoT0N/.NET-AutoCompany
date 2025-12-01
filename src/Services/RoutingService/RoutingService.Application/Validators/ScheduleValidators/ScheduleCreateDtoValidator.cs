using RoutingService.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.Bll.Validators.ScheduleValidators
{
    public class CreateScheduleDtoValidator : AbstractValidator<CreateScheduleDto>
    {
        public CreateScheduleDtoValidator()
        {
            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Route ID must be greater than 0");

            RuleFor(x => x.DepartureTime)
                .NotEmpty().WithMessage("Departure time is required")
                .Must(BeValidTime).WithMessage("Departure time must be between 00:00 and 23:59");

            RuleFor(x => x.ArrivalTime)
                .NotEmpty().WithMessage("Arrival time is required")
                .Must(BeValidTime).WithMessage("Arrival time must be between 00:00 and 23:59");

            RuleFor(x => x)
                .Must(x => x.ArrivalTime > x.DepartureTime)
                .WithMessage("Arrival time must be after departure time")
                .When(x => BeValidTime(x.DepartureTime) && BeValidTime(x.ArrivalTime));
        }

        private bool BeValidTime(TimeSpan time)
        {
            return time >= TimeSpan.Zero && time < TimeSpan.FromHours(24);
        }
    }
}
