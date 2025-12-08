using FluentValidation;
using RoutingService.Bll.DTOs;

namespace RoutingService.Bll.Validators.RouteValidators
{
    public class CreateRouteDtoValidator : AbstractValidator<CreateRouteDto>
    {
        public CreateRouteDtoValidator()
        {
            RuleFor(x => x.RouteNumber)
                .NotEmpty().WithMessage("Route number is required")
                .MaximumLength(20).WithMessage("Route number cannot exceed 20 characters")
                .Matches("^[A-Z0-9-]+$").WithMessage("Route number must contain only uppercase letters, numbers, and hyphens");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Route name is required")
                .MaximumLength(150).WithMessage("Route name cannot exceed 150 characters")
                .MinimumLength(3).WithMessage("Route name must be at least 3 characters");

            RuleFor(x => x.DistanceKm)
                .GreaterThan(0).WithMessage("Distance must be greater than 0")
                .LessThanOrEqualTo(9999.99m).WithMessage("Distance cannot exceed 9999.99 km");
        }
    }
}