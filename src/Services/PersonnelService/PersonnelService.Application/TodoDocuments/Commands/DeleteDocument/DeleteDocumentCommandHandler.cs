using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Commands.DeleteDocument
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, string>
    {
        private readonly IDocumentRepository _repository;
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;

        public DeleteDocumentCommandHandler(
            IDocumentRepository repository,
            ILogger<DeleteDocumentCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting document with Id: {Id}", request.Id);

            var document = await _repository.GetByIdAsync(request.Id);
            if (document == null)
                throw new NotFoundException("Document", request.Id);

            await _repository.DeleteAsync(request.Id);

            _logger.LogInformation("Document with Id {Id} deleted successfully", request.Id);
            return $"Document '{request.Id}' deleted successfully.";
        }
    }
}