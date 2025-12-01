using FluentValidation;
using RoutingService.Core.DTOs;
using System;

namespace RoutingService.Bll.Validators.RouteSheetValidators
{
    public class CreateRouteSheetDtoValidator : AbstractValidator<CreateRouteSheetDto>
    {
        public CreateRouteSheetDtoValidator()
        {
            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Route ID must be greater than 0");

            RuleFor(x => x.BusId)
                .GreaterThan(0).WithMessage("Bus ID must be greater than 0");

            RuleFor(x => x.SheetDate)
                .NotEmpty().WithMessage("Sheet date is required")
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30)).WithMessage("Sheet date cannot be more than 30 days in the past")
                .LessThanOrEqualTo(DateTime.Today.AddDays(365)).WithMessage("Sheet date cannot be more than 1 year in the future");
        }
    }
}