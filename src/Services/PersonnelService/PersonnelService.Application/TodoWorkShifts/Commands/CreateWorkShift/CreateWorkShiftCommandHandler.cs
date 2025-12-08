using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using PersonnelService.Application.Common.Exceptions;

namespace PersonnelService.Application.TodoWorkShifts.Commands.CreateWorkShift
{
    public class CreateWorkShiftCommandHandler : IRequestHandler<CreateWorkShiftCommand, string>
    {
        private readonly IWorkShiftRepository _repository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly ILogger<CreateWorkShiftCommandHandler> _logger;

        public CreateWorkShiftCommandHandler(
            IWorkShiftRepository repository,
            IPersonnelRepository personnelRepository,
            ILogger<CreateWorkShiftCommandHandler> logger)
        {
            _repository = repository;
            _personnelRepository = personnelRepository;
            _logger = logger;
        }

        public async Task<string> Handle(CreateWorkShiftCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating work shift for PersonnelId: {PersonnelId}", request.PersonnelId);

            // Перевірка чи існує персонал
            var personnelExists = await _personnelRepository.ExistsByPersonnelIdAsync(request.PersonnelId, cancellationToken);
            if (!personnelExists)
                throw new NotFoundException("Personnel", request.PersonnelId.ToString());

            var busInfo = new BusInfoVO(
                busCountryNumber: request.BusCountryNumber,
                brand: request.BusBrand
            );

            var routeInfo = new RouteInfoVO(
                routeNumber: request.RouteNumber,
                distanceKm: request.DistanceKm
            );

            var workShift = new WorkShiftLog(
                personnelId: request.PersonnelId,
                shiftDate: request.ShiftDate,
                startTime: request.StartTime,
                endTime: request.EndTime,
                bus: busInfo,
                route: routeInfo
            );

            await _repository.AddAsync(workShift);

            _logger.LogInformation("Work shift created with Id: {Id}", workShift.Id);
            return workShift.Id!;
        }
    }
}