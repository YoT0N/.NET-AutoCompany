using FluentValidation;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Commands.CreateExamination
{
    public class CreateExaminationCommandValidator : AbstractValidator<CreateExaminationCommand>
    {
        public CreateExaminationCommandValidator(IPersonnelRepository personnelRepository)
        {
            RuleFor(x => x.PersonnelId)
                .GreaterThan(0).WithMessage("PersonnelId must be positive.")
                .MustAsync(async (personnelId, cancellation) =>
                    await personnelRepository.ExistsByPersonnelIdAsync(personnelId, cancellation))
                .WithMessage("Personnel with this ID does not exist.");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("Examination date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Examination date cannot be in the future.");

            RuleFor(x => x.Result)
                .NotEmpty().WithMessage("Examination result is required.")
                .Must(result => new[] { "Passed", "Failed", "Pending" }.Contains(result))
                .WithMessage("Result must be one of: Passed, Failed, Pending.");

            RuleFor(x => x.DoctorName)
                .NotEmpty().WithMessage("Doctor name is required.")
                .MinimumLength(3).WithMessage("Doctor name must be at least 3 characters.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be positive.")
                .LessThanOrEqualTo(250).WithMessage("Height cannot exceed 250 cm.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be positive.")
                .LessThanOrEqualTo(300).WithMessage("Weight cannot exceed 300 kg.");

            RuleFor(x => x.BloodPressure)
                .NotEmpty().WithMessage("Blood pressure is required.")
                .Matches(@"^\d{2,3}/\d{2,3}$").WithMessage("Blood pressure must be in format XXX/YYY.");

            RuleFor(x => x.Vision)
                .NotEmpty().WithMessage("Vision is required.")
                .Matches(@"^[0-2]\.\d/[0-2]\.\d$").WithMessage("Vision must be in format X.X/Y.Y.");
        }
    }
}