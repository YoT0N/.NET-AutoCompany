using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Application.Common.Mappings;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelById
{
    public class GetPersonnelByIdQueryHandler : IRequestHandler<GetPersonnelByIdQuery, PersonnelDto>
    {
        private readonly IPersonnelRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPersonnelByIdQueryHandler> _logger;

        public GetPersonnelByIdQueryHandler(
            IPersonnelRepository repository,
            IMapper mapper,
            ILogger<GetPersonnelByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PersonnelDto> Handle(GetPersonnelByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching personnel by Id: {Id}", request.Id);

            var personnel = await _repository.GetByIdAsync(request.Id);
            if (personnel == null)
                throw new NotFoundException($"Personnel with ID '{request.Id}' not found.");

            _logger.LogInformation("Personnel with Id {Id} retrieved successfully", request.Id);
            return _mapper.Map<PersonnelDto>(personnel);
        }
    }
}