using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelByPosition
{
    public class GetPersonnelByPositionQueryHandler
        : IRequestHandler<GetPersonnelByPositionQuery, IReadOnlyCollection<PersonnelDto>>
    {
        private readonly IPersonnelRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPersonnelByPositionQueryHandler> _logger;

        public GetPersonnelByPositionQueryHandler(
            IPersonnelRepository repository,
            IMapper mapper,
            ILogger<GetPersonnelByPositionQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<PersonnelDto>> Handle(
            GetPersonnelByPositionQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching personnel by position: {Position}", request.Position);

            var personnel = await _repository.GetByPositionAsync(request.Position);
            var dtos = _mapper.Map<IReadOnlyCollection<PersonnelDto>>(personnel);

            _logger.LogInformation("Fetched {Count} personnel with position {Position}",
                dtos.Count, request.Position);

            return dtos;
        }
    }
}
