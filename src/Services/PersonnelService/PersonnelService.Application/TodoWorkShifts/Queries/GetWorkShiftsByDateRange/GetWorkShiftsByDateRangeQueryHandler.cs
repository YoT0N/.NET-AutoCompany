using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByDateRange
{
    public class GetWorkShiftsByDateRangeQueryHandler
        : IRequestHandler<GetWorkShiftsByDateRangeQuery, IReadOnlyCollection<WorkShiftDto>>
    {
        private readonly IWorkShiftRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetWorkShiftsByDateRangeQueryHandler> _logger;

        public GetWorkShiftsByDateRangeQueryHandler(
            IWorkShiftRepository repository,
            IMapper mapper,
            ILogger<GetWorkShiftsByDateRangeQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<WorkShiftDto>> Handle(
            GetWorkShiftsByDateRangeQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching work shifts from {StartDate} to {EndDate} for PersonnelId: {PersonnelId}",
                request.StartDate, request.EndDate, request.PersonnelId);

            var workShifts = request.PersonnelId.HasValue
                ? await _repository.GetByPersonnelAndDateRangeAsync(
                    request.PersonnelId.Value,
                    request.StartDate,
                    request.EndDate)
                : await _repository.GetByDateRangeAsync(request.StartDate, request.EndDate);

            var dtos = _mapper.Map<IReadOnlyCollection<WorkShiftDto>>(workShifts);

            _logger.LogInformation("Fetched {Count} work shifts", dtos.Count);
            return dtos;
        }
    }
}