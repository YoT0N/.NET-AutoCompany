using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Queries.GetExaminationById
{
    public class GetExaminationByIdQueryHandler : IRequestHandler<GetExaminationByIdQuery, ExaminationDto>
    {
        private readonly IExaminationRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetExaminationByIdQueryHandler> _logger;

        public GetExaminationByIdQueryHandler(
            IExaminationRepository repository,
            IMapper mapper,
            ILogger<GetExaminationByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ExaminationDto> Handle(GetExaminationByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching examination by Id: {Id}", request.Id);

            var examination = await _repository.GetByIdAsync(request.Id);
            if (examination == null)
                throw new NotFoundException($"Examination with ID '{request.Id}' not found.");

            _logger.LogInformation("Examination with Id {Id} retrieved successfully", request.Id);
            return _mapper.Map<ExaminationDto>(examination);
        }
    }
}