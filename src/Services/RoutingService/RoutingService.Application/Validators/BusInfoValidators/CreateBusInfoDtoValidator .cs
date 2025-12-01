using FluentValidation;
using RoutingService.Core.DTOs;

namespace RoutingService.Bll.Validators.BusInfoValidators
{
    public class CreateBusInfoDtoValidator : AbstractValidator<CreateBusInfoDto>
    {
        public CreateBusInfoDtoValidator()
        {
            RuleFor(x => x.CountryNumber)
                .NotEmpty().WithMessage("Country number is required")
                .MaximumLength(50).WithMessage("Country number cannot exceed 50 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Country number must contain only uppercase letters and numbers");

            RuleFor(x => x.Brand)
                .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Brand));

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0")
                .LessThanOrEqualTo(300).WithMessage("Capacity cannot exceed 300")
                .When(x => x.Capacity.HasValue);

            RuleFor(x => x.YearOfManufacture)
                .GreaterThanOrEqualTo(1900).WithMessage("Year of manufacture must be 1900 or later")
                .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year of manufacture cannot be in the future")
                .When(x => x.YearOfManufacture.HasValue);
        }
    }
}