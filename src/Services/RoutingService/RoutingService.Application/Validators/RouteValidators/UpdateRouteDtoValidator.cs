using FluentValidation;
using RoutingService.Core.DTOs;

namespace RoutingService.Bll.Validators.RouteValidators
{
    public class UpdateRouteDtoValidator : AbstractValidator<UpdateRouteDto>
    {
        public UpdateRouteDtoValidator()
        {
            When(x => x.RouteNumber != null, () =>
            {
                RuleFor(x => x.RouteNumber)
                    .MaximumLength(20).WithMessage("Route number cannot exceed 20 characters")
                    .Matches("^[A-Z0-9-]+$").WithMessage("Route number must contain only uppercase letters, numbers, and hyphens");
            });

            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .MaximumLength(150).WithMessage("Route name cannot exceed 150 characters")
                    .MinimumLength(3).WithMessage("Route name must be at least 3 characters");
            });

            When(x => x.DistanceKm.HasValue, () =>
            {
                RuleFor(x => x.DistanceKm)
                    .GreaterThan(0).WithMessage("Distance must be greater than 0")
                    .LessThanOrEqualTo(9999.99m).WithMessage("Distance cannot exceed 9999.99 km");
            });
        }
    }
}