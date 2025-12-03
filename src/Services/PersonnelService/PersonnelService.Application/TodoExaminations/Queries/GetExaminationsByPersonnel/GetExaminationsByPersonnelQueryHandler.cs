using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Queries.GetExaminationsByPersonnel
{
    public class GetExaminationsByPersonnelQueryHandler
        : IRequestHandler<GetExaminationsByPersonnelQuery, IReadOnlyCollection<ExaminationDto>>
    {
        private readonly IExaminationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetExaminationsByPersonnelQueryHandler> _logger;

        public GetExaminationsByPersonnelQueryHandler(
            IExaminationRepository repository,
            IMapper mapper,
            ILogger<GetExaminationsByPersonnelQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<ExaminationDto>> Handle(
            GetExaminationsByPersonnelQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching examinations for PersonnelId: {PersonnelId}", request.PersonnelId);

            var examinations = await _repository.GetByPersonnelIdAsync(request.PersonnelId);
            var dtos = _mapper.Map<IReadOnlyCollection<ExaminationDto>>(examinations);

            _logger.LogInformation("Fetched {Count} examinations for PersonnelId {PersonnelId}",
                dtos.Count, request.PersonnelId);

            return dtos;
        }
    }
}