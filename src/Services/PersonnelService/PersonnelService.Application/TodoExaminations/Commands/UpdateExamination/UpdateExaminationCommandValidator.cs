using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoExaminations.Commands.UpdateExamination
{
    public class UpdateExaminationCommandValidator : AbstractValidator<UpdateExaminationCommand>
    {
        public UpdateExaminationCommandValidator(IExaminationRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Examination Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Examination not found.");

            When(x => !string.IsNullOrWhiteSpace(x.Result), () =>
            {
                RuleFor(x => x.Result)
                    .Must(result => new[] { "Passed", "Failed", "Pending" }.Contains(result!))
                    .WithMessage("Result must be one of: Passed, Failed, Pending.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.DoctorName), () =>
            {
                RuleFor(x => x.DoctorName)
                    .MinimumLength(3).WithMessage("Doctor name must be at least 3 characters.");
            });

            When(x => x.Height.HasValue, () =>
            {
                RuleFor(x => x.Height!.Value)
                    .GreaterThan(0).WithMessage("Height must be positive.")
                    .LessThanOrEqualTo(250).WithMessage("Height cannot exceed 250 cm.");
            });

            When(x => x.Weight.HasValue, () =>
            {
                RuleFor(x => x.Weight!.Value)
                    .GreaterThan(0).WithMessage("Weight must be positive.")
                    .LessThanOrEqualTo(300).WithMessage("Weight cannot exceed 300 kg.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.BloodPressure), () =>
            {
                RuleFor(x => x.BloodPressure)
                    .Matches(@"^\d{2,3}/\d{2,3}$").WithMessage("Blood pressure must be in format XXX/YYY.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Vision), () =>
            {
                RuleFor(x => x.Vision)
                    .Matches(@"^[0-2]\.\d/[0-2]\.\d$").WithMessage("Vision must be in format X.X/Y.Y.");
            });
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}