using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Application.Common.Exceptions;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Application.TodoPersonnel.Commands.UpdatePersonnel
{
    public class UpdatePersonnelCommandHandler : IRequestHandler<UpdatePersonnelCommand, string>
    {
        private readonly IPersonnelRepository _repository;
        private readonly ILogger<UpdatePersonnelCommandHandler> _logger;

        public UpdatePersonnelCommandHandler(
            IPersonnelRepository repository,
            ILogger<UpdatePersonnelCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdatePersonnelCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating personnel with Id: {Id}", request.Id);

            var personnel = await _repository.GetByIdAsync(request.Id);
            if (personnel == null)
                throw new NotFoundException("Personnel", request.Id);

            // Оновлюємо персональну інформацію
            if (!string.IsNullOrWhiteSpace(request.FullName) ||
                request.BirthDate.HasValue ||
                !string.IsNullOrWhiteSpace(request.Position))
            {
                personnel.UpdatePersonalInfo(
                    request.FullName ?? personnel.FullName,
                    request.BirthDate ?? personnel.BirthDate,
                    request.Position ?? personnel.Position
                );
            }

            // Оновлюємо контакти
            if (!string.IsNullOrWhiteSpace(request.Phone) ||
                !string.IsNullOrWhiteSpace(request.Email) ||
                !string.IsNullOrWhiteSpace(request.Address))
            {
                var contacts = new PersonnelContactsVO(
                    request.Phone ?? personnel.Contacts.Phone,
                    request.Email ?? personnel.Contacts.Email,
                    request.Address ?? personnel.Contacts.Address
                );
                personnel.UpdateContacts(contacts);
            }

            await _repository.UpdateAsync(personnel);

            _logger.LogInformation("Personnel with Id {Id} updated successfully", personnel.Id);
            return $"Personnel '{personnel.Id}' updated successfully.";
        }
    }
}