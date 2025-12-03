using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoExaminations.Commands.DeleteExamination
{
    public class DeleteExaminationCommandHandler : IRequestHandler<DeleteExaminationCommand, string>
    {
        private readonly IExaminationRepository _repository;
        private readonly ILogger<DeleteExaminationCommandHandler> _logger;

        public DeleteExaminationCommandHandler(
            IExaminationRepository repository,
            ILogger<DeleteExaminationCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(DeleteExaminationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting examination with Id: {Id}", request.Id);

            var examination = await _repository.GetByIdAsync(request.Id);
            if (examination == null)
                throw new NotFoundException("Examination", request.Id);

            await _repository.DeleteAsync(request.Id);

            _logger.LogInformation("Examination with Id {Id} deleted successfully", request.Id);
            return $"Examination '{request.Id}' deleted successfully.";
        }
    }
}