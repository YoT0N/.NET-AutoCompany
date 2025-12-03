using FluentValidation;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Commands.CreateDocument
{
    public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
    {
        public CreateDocumentCommandValidator(IPersonnelRepository personnelRepository)
        {
            RuleFor(x => x.PersonnelId)
                .GreaterThan(0).WithMessage("PersonnelId must be positive.")
                .MustAsync(async (personnelId, cancellation) =>
                    await personnelRepository.ExistsByPersonnelIdAsync(personnelId, cancellation))
                .WithMessage("Personnel with this ID does not exist.");

            RuleFor(x => x.DocType)
                .NotEmpty().WithMessage("Document type is required.")
                .MaximumLength(100).WithMessage("Document type cannot exceed 100 characters.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters.");

            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("File size must be positive.")
                .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size cannot exceed 10 MB.");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("MIME type is required.")
                .MaximumLength(100).WithMessage("MIME type cannot exceed 100 characters.");

            When(x => x.ValidUntil.HasValue && x.IssuedOn.HasValue, () =>
            {
                RuleFor(x => x.ValidUntil!.Value)
                    .GreaterThan(x => x.IssuedOn!.Value)
                    .WithMessage("Valid until date must be after issued on date.");
            });
        }
    }
}