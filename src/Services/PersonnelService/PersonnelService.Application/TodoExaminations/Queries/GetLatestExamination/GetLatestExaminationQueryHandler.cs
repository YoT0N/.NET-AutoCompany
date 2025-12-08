using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Queries.GetLatestExamination
{
    public class GetLatestExaminationQueryHandler : IRequestHandler<GetLatestExaminationQuery, ExaminationDto?>
    {
        private readonly IExaminationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLatestExaminationQueryHandler> _logger;

        public GetLatestExaminationQueryHandler(
            IExaminationRepository repository,
            IMapper mapper,
            ILogger<GetLatestExaminationQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ExaminationDto?> Handle(GetLatestExaminationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching latest examination for PersonnelId: {PersonnelId}", request.PersonnelId);

            var examination = await _repository.GetLatestByPersonnelIdAsync(request.PersonnelId);

            if (examination == null)
            {
                _logger.LogInformation("No examinations found for PersonnelId {PersonnelId}", request.PersonnelId);
                return null;
            }

            _logger.LogInformation("Latest examination retrieved for PersonnelId {PersonnelId}", request.PersonnelId);
            return _mapper.Map<ExaminationDto>(examination);
        }
    }
}