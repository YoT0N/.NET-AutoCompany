using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Application.Common.Exceptions;

namespace PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnelStatus
{
    public class UpdatePersonnelStatusCommandHandler : IRequestHandler<UpdatePersonnelStatusCommand, string>
    {
        private readonly IPersonnelRepository _repository;
        private readonly ILogger<UpdatePersonnelStatusCommandHandler> _logger;

        public UpdatePersonnelStatusCommandHandler(
            IPersonnelRepository repository,
            ILogger<UpdatePersonnelStatusCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdatePersonnelStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating personnel status for Id: {Id}", request.Id);

            var personnel = await _repository.GetByIdAsync(request.Id);
            if (personnel == null)
                throw new NotFoundException("Personnel", request.Id);

            personnel.UpdateStatus(request.Status);

            await _repository.UpdateAsync(personnel);

            _logger.LogInformation("Personnel status updated to {Status} for Id {Id}",
                request.Status, request.Id);

            return $"Personnel '{request.Id}' status updated to '{request.Status}'.";
        }
    }
}