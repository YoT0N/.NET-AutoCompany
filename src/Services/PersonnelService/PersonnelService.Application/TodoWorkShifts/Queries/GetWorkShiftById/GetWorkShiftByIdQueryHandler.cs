using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftById
{
    public class GetWorkShiftByIdQueryHandler : IRequestHandler<GetWorkShiftByIdQuery, WorkShiftDto>
    {
        private readonly IWorkShiftRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetWorkShiftByIdQueryHandler> _logger;

        public GetWorkShiftByIdQueryHandler(
            IWorkShiftRepository repository,
            IMapper mapper,
            ILogger<GetWorkShiftByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WorkShiftDto> Handle(GetWorkShiftByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching work shift by Id: {Id}", request.Id);

            var workShift = await _repository.GetByIdAsync(request.Id);
            if (workShift == null)
                throw new NotFoundException($"Work shift with ID '{request.Id}' not found.");

            _logger.LogInformation("Work shift with Id {Id} retrieved successfully", request.Id);
            return _mapper.Map<WorkShiftDto>(workShift);
        }
    }
}