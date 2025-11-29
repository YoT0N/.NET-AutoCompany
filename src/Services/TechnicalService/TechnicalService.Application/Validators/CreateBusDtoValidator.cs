using FluentValidation;
using TechnicalService.Bll.DTOs.Bus;

namespace TechnicalService.Bll.Validators;

public class CreateBusDtoValidator : AbstractValidator<CreateBusDto>
{
    public CreateBusDtoValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Модель автобуса є обов'язковою")
            .MaximumLength(100).WithMessage("Модель не може перевищувати 100 символів");

        RuleFor(x => x.BoardingNumber)
            .NotEmpty().WithMessage("Номерний знак є обов'язковим")
            .MaximumLength(20).WithMessage("Номерний знак не може перевищувати 20 символів")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Номерний знак може містити тільки великі літери та цифри");

        RuleFor(x => x.YearOfManufacture)
            .GreaterThan(1900).WithMessage("Рік випуску має бути більшим за 1900")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Рік випуску не може бути в майбутньому");

        RuleFor(x => x.PassengerCapacity)
            .GreaterThan(0).WithMessage("Місткість має бути більшою за 0")
            .LessThanOrEqualTo(300).WithMessage("Місткість не може перевищувати 300 пасажирів");

        RuleFor(x => x.CurrentStatusId)
            .NotEmpty().WithMessage("Статус є обов'язковим");
    }
}