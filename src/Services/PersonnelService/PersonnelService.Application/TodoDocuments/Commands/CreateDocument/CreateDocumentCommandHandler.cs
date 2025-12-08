using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Application.Common.Exceptions;

namespace PersonnelService.Application.TodoDocuments.Commands.CreateDocument
{
    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, string>
    {
        private readonly IDocumentRepository _repository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly ILogger<CreateDocumentCommandHandler> _logger;

        public CreateDocumentCommandHandler(
            IDocumentRepository repository,
            IPersonnelRepository personnelRepository,
            ILogger<CreateDocumentCommandHandler> logger)
        {
            _repository = repository;
            _personnelRepository = personnelRepository;
            _logger = logger;
        }

        public async Task<string> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating document for PersonnelId: {PersonnelId}", request.PersonnelId);

            // Перевірка чи існує персонал
            var personnelExists = await _personnelRepository.ExistsByPersonnelIdAsync(request.PersonnelId, cancellationToken);
            if (!personnelExists)
                throw new NotFoundException("Personnel", request.PersonnelId.ToString());

            var document = new PersonnelDocument(
                personnelId: request.PersonnelId,
                docType: request.DocType,
                fileName: request.FileName,
                fileSize: request.FileSize,
                mimeType: request.MimeType,
                issuedOn: request.IssuedOn,
                validUntil: request.ValidUntil
            );

            // Додаткові деталі документа
            if (!string.IsNullOrWhiteSpace(request.Category) ||
                !string.IsNullOrWhiteSpace(request.IssuedBy) ||
                !string.IsNullOrWhiteSpace(request.DocumentNumber))
            {
                document.UpdateDocumentDetails(
                    issuedOn: request.IssuedOn,
                    validUntil: request.ValidUntil,
                    category: request.Category,
                    issuedBy: request.IssuedBy,
                    documentNumber: request.DocumentNumber
                );
            }

            await _repository.AddAsync(document);

            _logger.LogInformation("Document created with Id: {Id}", document.Id);
            return document.Id!;
        }
    }
}