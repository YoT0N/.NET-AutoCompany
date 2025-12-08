using FluentValidation;
using RoutingService.Bll.DTOs;

namespace RoutingService.Bll.Validators.BusInfoValidators
{
    public class UpdateBusInfoDtoValidator : AbstractValidator<UpdateBusInfoDto>
    {
        public UpdateBusInfoDtoValidator()
        {
            When(x => x.CountryNumber != null, () =>
            {
                RuleFor(x => x.CountryNumber)
                    .MaximumLength(50).WithMessage("Country number cannot exceed 50 characters")
                    .Matches("^[A-Z0-9]+$").WithMessage("Country number must contain only uppercase letters and numbers");
            });

            When(x => x.Brand != null, () =>
            {
                RuleFor(x => x.Brand)
                    .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters");
            });

            When(x => x.Capacity.HasValue, () =>
            {
                RuleFor(x => x.Capacity)
                    .GreaterThan(0).WithMessage("Capacity must be greater than 0")
                    .LessThanOrEqualTo(300).WithMessage("Capacity cannot exceed 300");
            });

            When(x => x.YearOfManufacture.HasValue, () =>
            {
                RuleFor(x => x.YearOfManufacture)
                    .GreaterThanOrEqualTo(1900).WithMessage("Year of manufacture must be 1900 or later")
                    .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year of manufacture cannot be in the future");
            });
        }
    }
}