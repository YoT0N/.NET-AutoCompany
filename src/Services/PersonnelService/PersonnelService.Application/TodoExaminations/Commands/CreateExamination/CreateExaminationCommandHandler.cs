using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using PersonnelService.Application.Common.Exceptions;

namespace PersonnelService.Application.TodoExaminations.Commands.CreateExamination
{
    public class CreateExaminationCommandHandler : IRequestHandler<CreateExaminationCommand, string>
    {
        private readonly IExaminationRepository _repository;
        private readonly IPersonnelRepository _personnelRepository;
        private readonly ILogger<CreateExaminationCommandHandler> _logger;

        public CreateExaminationCommandHandler(
            IExaminationRepository repository,
            IPersonnelRepository personnelRepository,
            ILogger<CreateExaminationCommandHandler> logger)
        {
            _repository = repository;
            _personnelRepository = personnelRepository;
            _logger = logger;
        }

        public async Task<string> Handle(CreateExaminationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating examination for PersonnelId: {PersonnelId}", request.PersonnelId);

            // Перевірка чи існує персонал
            var personnelExists = await _personnelRepository.ExistsByPersonnelIdAsync(request.PersonnelId, cancellationToken);
            if (!personnelExists)
                throw new NotFoundException("Personnel", request.PersonnelId.ToString());

            var metrics = new ExaminationMetricsVO(
                height: request.Height,
                weight: request.Weight,
                bloodPressure: request.BloodPressure,
                vision: request.Vision
            );

            var examination = new PhysicalExamination(
                personnelId: request.PersonnelId,
                examDate: request.ExamDate,
                result: request.Result,
                doctorName: request.DoctorName,
                metrics: metrics,
                notes: request.Notes
            );

            await _repository.AddAsync(examination);

            _logger.LogInformation("Examination created with Id: {Id}", examination.Id);
            return examination.Id!;
        }
    }
}