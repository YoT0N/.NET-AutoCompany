using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoPersonnel.Commands.DeletePersonnel
{
    public class DeletePersonnelCommandHandler : IRequestHandler<DeletePersonnelCommand, string>
    {
        private readonly IPersonnelRepository _repository;
        private readonly ILogger<DeletePersonnelCommandHandler> _logger;

        public DeletePersonnelCommandHandler(
            IPersonnelRepository repository,
            ILogger<DeletePersonnelCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(DeletePersonnelCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting personnel with Id: {Id}", request.Id);

            var personnel = await _repository.GetByIdAsync(request.Id);
            if (personnel == null)
                throw new NotFoundException("Personnel", request.Id);

            await _repository.DeleteAsync(request.Id);

            _logger.LogInformation("Personnel with Id {Id} deleted successfully", request.Id);
            return $"Personnel '{request.Id}' deleted successfully.";
        }
    }
}