using FluentValidation;
using RoutingService.Core.DTOs;

namespace RoutingService.Bll.Validators.RouteStopValidators
{
    public class UpdateRouteStopDtoValidator : AbstractValidator<UpdateRouteStopDto>
    {
        public UpdateRouteStopDtoValidator()
        {
            When(x => x.StopName != null, () =>
            {
                RuleFor(x => x.StopName)
                    .MaximumLength(150).WithMessage("Stop name cannot exceed 150 characters")
                    .MinimumLength(2).WithMessage("Stop name must be at least 2 characters");
            });

            When(x => x.Latitude.HasValue, () =>
            {
                RuleFor(x => x.Latitude)
                    .InclusiveBetween(-90m, 90m).WithMessage("Latitude must be between -90 and 90");
            });

            When(x => x.Longitude.HasValue, () =>
            {
                RuleFor(x => x.Longitude)
                    .InclusiveBetween(-180m, 180m).WithMessage("Longitude must be between -180 and 180");
            });
        }
    }
}