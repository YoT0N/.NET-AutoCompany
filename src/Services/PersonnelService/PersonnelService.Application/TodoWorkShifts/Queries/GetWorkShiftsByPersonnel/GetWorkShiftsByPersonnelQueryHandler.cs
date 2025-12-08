using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByPersonnel
{
    public class GetWorkShiftsByPersonnelQueryHandler
        : IRequestHandler<GetWorkShiftsByPersonnelQuery, IReadOnlyCollection<WorkShiftDto>>
    {
        private readonly IWorkShiftRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetWorkShiftsByPersonnelQueryHandler> _logger;

        public GetWorkShiftsByPersonnelQueryHandler(
            IWorkShiftRepository repository,
            IMapper mapper,
            ILogger<GetWorkShiftsByPersonnelQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<WorkShiftDto>> Handle(
            GetWorkShiftsByPersonnelQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching work shifts for PersonnelId: {PersonnelId}", request.PersonnelId);

            var workShifts = await _repository.GetByPersonnelIdAsync(request.PersonnelId);
            var dtos = _mapper.Map<IReadOnlyCollection<WorkShiftDto>>(workShifts);

            _logger.LogInformation("Fetched {Count} work shifts for PersonnelId {PersonnelId}",
                dtos.Count, request.PersonnelId);

            return dtos;
        }
    }
}