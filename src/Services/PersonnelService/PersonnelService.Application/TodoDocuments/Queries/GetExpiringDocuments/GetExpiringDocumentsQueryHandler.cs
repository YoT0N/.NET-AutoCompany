using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoDocuments.Queries.GetExpiringDocuments
{
    public class GetExpiringDocumentsQueryHandler
        : IRequestHandler<GetExpiringDocumentsQuery, IReadOnlyCollection<DocumentDto>>
    {
        private readonly IDocumentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetExpiringDocumentsQueryHandler> _logger;

        public GetExpiringDocumentsQueryHandler(
            IDocumentRepository repository,
            IMapper mapper,
            ILogger<GetExpiringDocumentsQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<DocumentDto>> Handle(
            GetExpiringDocumentsQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching documents expiring within {Days} days", request.DaysThreshold);

            var documents = await _repository.GetExpiringDocumentsAsync(request.DaysThreshold);
            var dtos = _mapper.Map<IReadOnlyCollection<DocumentDto>>(documents);

            _logger.LogInformation("Found {Count} expiring documents", dtos.Count);
            return dtos;
        }
    }
}