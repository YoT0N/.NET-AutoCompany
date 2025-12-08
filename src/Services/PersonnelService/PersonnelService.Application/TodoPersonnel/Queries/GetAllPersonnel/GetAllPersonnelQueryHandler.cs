using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetAllPersonnel
{
    public class GetAllPersonnelQueryHandler : IRequestHandler<GetAllPersonnelQuery, IReadOnlyCollection<PersonnelDto>>
    {
        private readonly IPersonnelRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllPersonnelQueryHandler> _logger;

        public GetAllPersonnelQueryHandler(
            IPersonnelRepository repository,
            IMapper mapper,
            ILogger<GetAllPersonnelQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<PersonnelDto>> Handle(
            GetAllPersonnelQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching personnel list. SearchText: {SearchText}, Position: {Position}, Status: {Status}",
                request.SearchText, request.Position, request.Status);

            var personnel = await _repository.SearchAsync(
                searchText: request.SearchText,
                position: request.Position,
                status: request.Status,
                skip: request.Skip,
                limit: request.Limit
            );

            var dtos = _mapper.Map<IReadOnlyCollection<PersonnelDto>>(personnel);
            _logger.LogInformation("Fetched {Count} personnel records", dtos.Count);

            return dtos;
        }
    }
}
