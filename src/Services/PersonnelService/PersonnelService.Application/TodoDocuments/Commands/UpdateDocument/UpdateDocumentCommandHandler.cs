using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Commands.UpdateDocument
{
    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, string>
    {
        private readonly IDocumentRepository _repository;
        private readonly ILogger<UpdateDocumentCommandHandler> _logger;

        public UpdateDocumentCommandHandler(
            IDocumentRepository repository,
            ILogger<UpdateDocumentCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating document with Id: {Id}", request.Id);

            var document = await _repository.GetByIdAsync(request.Id);
            if (document == null)
                throw new NotFoundException("Document", request.Id);

            document.UpdateDocumentDetails(
                issuedOn: request.IssuedOn,
                validUntil: request.ValidUntil,
                category: request.Category,
                issuedBy: request.IssuedBy,
                documentNumber: request.DocumentNumber
            );

            await _repository.UpdateAsync(document);

            _logger.LogInformation("Document with Id {Id} updated successfully", request.Id);
            return $"Document '{request.Id}' updated successfully.";
        }
    }
}