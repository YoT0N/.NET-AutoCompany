using FluentValidation;
using RoutingService.Bll.DTOs;
using System;

namespace RoutingService.Bll.Validators.RouteSheetValidators
{
    public class UpdateRouteSheetDtoValidator : AbstractValidator<UpdateRouteSheetDto>
    {
        public UpdateRouteSheetDtoValidator()
        {
            When(x => x.RouteId.HasValue, () =>
            {
                RuleFor(x => x.RouteId)
                    .GreaterThan(0).WithMessage("Route ID must be greater than 0");
            });

            When(x => x.BusId.HasValue, () =>
            {
                RuleFor(x => x.BusId)
                    .GreaterThan(0).WithMessage("Bus ID must be greater than 0");
            });

            When(x => x.SheetDate.HasValue, () =>
            {
                RuleFor(x => x.SheetDate)
                    .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30))
                    .WithMessage("Sheet date cannot be more than 30 days in the past")
                    .LessThanOrEqualTo(DateTime.Today.AddDays(365))
                    .WithMessage("Sheet date cannot be more than 1 year in the future");
            });
        }
    }
}