using MediatR;
using Microsoft.Extensions.Logging;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Application.TodoPersonnel.Commands.CreatePersonnel
{
    public class CreatePersonnelCommandHandler : IRequestHandler<CreatePersonnelCommand, string>
    {
        private readonly IPersonnelRepository _repository;
        private readonly ILogger<CreatePersonnelCommandHandler> _logger;

        public CreatePersonnelCommandHandler(
            IPersonnelRepository repository,
            ILogger<CreatePersonnelCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(CreatePersonnelCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating personnel: {FullName}", request.FullName);

            var nextId = await _repository.GetNextPersonnelIdAsync();

            var contacts = new PersonnelContactsVO(
                request.Phone,
                request.Email,
                request.Address
            );

            var personnel = new Personnel(
                nextId,
                request.FullName,
                request.BirthDate,
                request.Position,
                contacts
            );

            await _repository.AddAsync(personnel);

            _logger.LogInformation("Personnel created with Id: {Id}, PersonnelId: {PersonnelId}",
                personnel.Id, personnel.PersonnelId);

            return personnel.Id!;
        }
    }
}