using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;

namespace PersonnelService.Application.TodoWorkShifts.Commands.DeleteWorkShift
{
    public class DeleteWorkShiftCommandHandler : IRequestHandler<DeleteWorkShiftCommand, string>
    {
        private readonly IWorkShiftRepository _repository;
        private readonly ILogger<DeleteWorkShiftCommandHandler> _logger;

        public DeleteWorkShiftCommandHandler(
            IWorkShiftRepository repository,
            ILogger<DeleteWorkShiftCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(DeleteWorkShiftCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting work shift with Id: {Id}", request.Id);

            var workShift = await _repository.GetByIdAsync(request.Id);
            if (workShift == null)
                throw new NotFoundException("WorkShift", request.Id);

            await _repository.DeleteAsync(request.Id);

            _logger.LogInformation("Work shift with Id {Id} deleted successfully", request.Id);
            return $"Work shift '{request.Id}' deleted successfully.";
        }
    }
}