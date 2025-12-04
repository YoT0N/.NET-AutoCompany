using FluentValidation;
using PersonnelService.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace PersonnelService.Application.TodoDocuments.Commands.UpdateDocument
{
    public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
    {
        public UpdateDocumentCommandValidator(IDocumentRepository repository)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Document Id is required.")
                .Must(IsValidObjectIdFormat).WithMessage("Invalid Id format.")
                .MustAsync(async (id, cancellation) => await repository.ExistsAsync(id, cancellation))
                .WithMessage("Document not found.");

            When(x => x.ValidUntil.HasValue && x.IssuedOn.HasValue, () =>
            {
                RuleFor(x => x.ValidUntil!.Value)
                    .GreaterThan(x => x.IssuedOn!.Value)
                    .WithMessage("Valid until date must be after issued on date.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Category), () =>
            {
                RuleFor(x => x.Category)
                    .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.IssuedBy), () =>
            {
                RuleFor(x => x.IssuedBy)
                    .MaximumLength(200).WithMessage("Issued by cannot exceed 200 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber), () =>
            {
                RuleFor(x => x.DocumentNumber)
                    .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters.");
            });
        }

        private static bool IsValidObjectIdFormat(string id)
        {
            return !string.IsNullOrEmpty(id) && Regex.IsMatch(id, "^[a-fA-F0-9]{24}$");
        }
    }
}