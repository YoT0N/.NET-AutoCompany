using FluentValidation;
using TechnicalService.Bll.DTOs.Examination;

namespace TechnicalService.Bll.Validators;

public class CreateExaminationDtoValidator : AbstractValidator<CreateExaminationDto>
{
    public CreateExaminationDtoValidator()
    {
        RuleFor(x => x.MechanicName)
            .NotEmpty().WithMessage("Ім'я екзаменатора є обов'язковим")
            .MaximumLength(200).WithMessage("Ім'я екзаменатора не може перевищувати 200 символів");

        RuleFor(x => x.ExaminationResult)
            .NotEmpty().WithMessage("Результат огляду є обов'язковим")
            .Must(x => x == "Пройдено" || x == "Не пройдено")
            .WithMessage("Результат має бути 'Пройдено' або 'Не пройдено'");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Коментарі не можуть перевищувати 1000 символів");
    }
}