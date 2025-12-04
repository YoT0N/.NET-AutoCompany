using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoExaminations.Commands.DeleteExamination
{
    public class DeleteExaminationCommandValidator : AbstractValidator<DeleteExaminationCommand>
    {
        public DeleteExaminationCommandValidator(IExaminationRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Examination Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Examination not found.");
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}