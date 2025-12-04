using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Application.TodoExaminations.Commands.UpdateExamination
{
    public class UpdateExaminationCommandHandler : IRequestHandler<UpdateExaminationCommand, string>
    {
        private readonly IExaminationRepository _repository;
        private readonly ILogger<UpdateExaminationCommandHandler> _logger;

        public UpdateExaminationCommandHandler(
            IExaminationRepository repository,
            ILogger<UpdateExaminationCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateExaminationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating examination with Id: {Id}", request.Id);

            var examination = await _repository.GetByIdAsync(request.Id);
            if (examination == null)
                throw new NotFoundException("Examination", request.Id);

            // Оновлюємо метрики якщо надано хоча б одне значення
            ExaminationMetricsVO metrics;
            if (request.Height.HasValue || request.Weight.HasValue ||
                !string.IsNullOrWhiteSpace(request.BloodPressure) ||
                !string.IsNullOrWhiteSpace(request.Vision))
            {
                metrics = new ExaminationMetricsVO(
                    height: request.Height ?? examination.Metrics.Height,
                    weight: request.Weight ?? examination.Metrics.Weight,
                    bloodPressure: request.BloodPressure ?? examination.Metrics.BloodPressure,
                    vision: request.Vision ?? examination.Metrics.Vision
                );
            }
            else
            {
                metrics = examination.Metrics;
            }

            examination.UpdateExamination(
                result: request.Result ?? examination.Result,
                doctorName: request.DoctorName ?? examination.DoctorName,
                metrics: metrics,
                notes: request.Notes
            );

            await _repository.UpdateAsync(examination);

            _logger.LogInformation("Examination with Id {Id} updated successfully", request.Id);
            return $"Examination '{request.Id}' updated successfully.";
        }
    }
}