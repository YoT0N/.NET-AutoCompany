using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Application.TodoWorkShifts.Commands.UpdateWorkShift
{
    public class UpdateWorkShiftCommandHandler : IRequestHandler<UpdateWorkShiftCommand, string>
    {
        private readonly IWorkShiftRepository _repository;
        private readonly ILogger<UpdateWorkShiftCommandHandler> _logger;

        public UpdateWorkShiftCommandHandler(
            IWorkShiftRepository repository,
            ILogger<UpdateWorkShiftCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateWorkShiftCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating work shift with Id: {Id}", request.Id);

            var workShift = await _repository.GetByIdAsync(request.Id);
            if (workShift == null)
                throw new NotFoundException("WorkShift", request.Id);

            // Оновлюємо основні деталі зміни якщо надано
            if (request.ShiftDate.HasValue ||
                !string.IsNullOrWhiteSpace(request.StartTime) ||
                !string.IsNullOrWhiteSpace(request.EndTime) ||
                !string.IsNullOrWhiteSpace(request.BusCountryNumber) ||
                !string.IsNullOrWhiteSpace(request.RouteNumber))
            {
                var busInfo = new BusInfoVO(
                    busCountryNumber: request.BusCountryNumber ?? workShift.Bus.BusCountryNumber,
                    brand: request.BusBrand ?? workShift.Bus.Brand
                );

                var routeInfo = new RouteInfoVO(
                    routeNumber: request.RouteNumber ?? workShift.Route.RouteNumber,
                    distanceKm: request.DistanceKm ?? workShift.Route.DistanceKm
                );

                workShift.UpdateShiftDetails(
                    shiftDate: request.ShiftDate ?? workShift.ShiftDate,
                    startTime: request.StartTime ?? workShift.StartTime,
                    endTime: request.EndTime ?? workShift.EndTime,
                    bus: busInfo,
                    route: routeInfo
                );
            }

            // Оновлюємо статус якщо надано
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                workShift.UpdateStatus(request.Status);
            }

            await _repository.UpdateAsync(workShift);

            _logger.LogInformation("Work shift with Id {Id} updated successfully", request.Id);
            return $"Work shift '{request.Id}' updated successfully.";
        }
    }
}