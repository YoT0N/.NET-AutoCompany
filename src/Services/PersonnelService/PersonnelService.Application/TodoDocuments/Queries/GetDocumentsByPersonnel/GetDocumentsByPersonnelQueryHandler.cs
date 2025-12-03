using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Queries.GetDocumentsByPersonnel
{
    public class GetDocumentsByPersonnelQueryHandler
        : IRequestHandler<GetDocumentsByPersonnelQuery, IReadOnlyCollection<DocumentDto>>
    {
        private readonly IDocumentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentsByPersonnelQueryHandler> _logger;

        public GetDocumentsByPersonnelQueryHandler(
            IDocumentRepository repository,
            IMapper mapper,
            ILogger<GetDocumentsByPersonnelQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<DocumentDto>> Handle(
            GetDocumentsByPersonnelQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching documents for PersonnelId: {PersonnelId}", request.PersonnelId);

            var documents = await _repository.GetByPersonnelIdAsync(request.PersonnelId);
            var dtos = _mapper.Map<IReadOnlyCollection<DocumentDto>>(documents);

            _logger.LogInformation("Fetched {Count} documents for PersonnelId {PersonnelId}",
                dtos.Count, request.PersonnelId);

            return dtos;
        }
    }
}