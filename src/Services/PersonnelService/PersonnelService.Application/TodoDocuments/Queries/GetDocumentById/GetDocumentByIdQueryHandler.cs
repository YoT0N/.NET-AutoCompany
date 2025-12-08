using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Queries.GetDocumentById
{
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto>
    {
        private readonly IDocumentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentByIdQueryHandler> _logger;

        public GetDocumentByIdQueryHandler(
            IDocumentRepository repository,
            IMapper mapper,
            ILogger<GetDocumentByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching document by Id: {Id}", request.Id);

            var document = await _repository.GetByIdAsync(request.Id);
            if (document == null)
                throw new NotFoundException($"Document with ID '{request.Id}' not found.");

            _logger.LogInformation("Document with Id {Id} retrieved successfully", request.Id);
            return _mapper.Map<DocumentDto>(document);
        }
    }
}