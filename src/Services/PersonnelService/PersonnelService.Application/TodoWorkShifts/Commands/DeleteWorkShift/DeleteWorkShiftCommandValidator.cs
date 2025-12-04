using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoWorkShifts.Commands.DeleteWorkShift
{
    public class DeleteWorkShiftCommandValidator : AbstractValidator<DeleteWorkShiftCommand>
    {
        public DeleteWorkShiftCommandValidator(IWorkShiftRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Work shift Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Work shift not found.");
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}