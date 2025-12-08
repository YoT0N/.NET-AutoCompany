using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnelStatus
{
    public class UpdatePersonnelStatusCommandValidator : AbstractValidator<UpdatePersonnelStatusCommand>
    {
        public UpdatePersonnelStatusCommandValidator(IPersonnelRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Personnel Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Personnel not found.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => new[] { "Active", "Inactive", "OnLeave", "Terminated" }.Contains(status))
                .WithMessage("Status must be one of: Active, Inactive, OnLeave, Terminated.");
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}