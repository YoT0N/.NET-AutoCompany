using PersonnelService.Application.Interfaces;
using PersonnelService.Core.DTOs;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;

namespace PersonnelService.Application.Services
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IPersonnelRepository _personnelRepository;

        public PersonnelService(IPersonnelRepository personnelRepository)
        {
            _personnelRepository = personnelRepository;
        }

        public async Task<IEnumerable<PersonnelDto>> GetAllPersonnelAsync()
        {
            var personnel = await _personnelRepository.GetAllAsync();
            return personnel.Select(MapToDto);
        }

        public async Task<PersonnelDto?> GetPersonnelByIdAsync(string id)
        {
            var personnel = await _personnelRepository.GetByIdAsync(id);
            return personnel != null ? MapToDto(personnel) : null;
        }

        public async Task<PersonnelDto?> GetPersonnelByPersonnelIdAsync(int personnelId)
        {
            var personnel = await _personnelRepository.GetByPersonnelIdAsync(personnelId);
            return personnel != null ? MapToDto(personnel) : null;
        }

        public async Task<IEnumerable<PersonnelDto>> GetPersonnelByPositionAsync(string position)
        {
            var personnel = await _personnelRepository.GetByPositionAsync(position);
            return personnel.Select(MapToDto);
        }

        public async Task<IEnumerable<PersonnelDto>> GetPersonnelByStatusAsync(string status)
        {
            var personnel = await _personnelRepository.GetByStatusAsync(status);
            return personnel.Select(MapToDto);
        }

        public async Task<IEnumerable<PersonnelDto>> GetActivePersonnelAsync()
        {
            var personnel = await _personnelRepository.GetActivePersonnelAsync();
            return personnel.Select(MapToDto);
        }

        public async Task<PersonnelDto> CreatePersonnelAsync(CreatePersonnelDto createDto)
        {
            var personnelId = await _personnelRepository.GetNextPersonnelIdAsync();

            var personnel = new Personnel
            {
                PersonnelId = personnelId,
                FullName = createDto.FullName,
                BirthDate = createDto.BirthDate,
                Position = createDto.Position,
                Status = "Active",
                Contacts = new PersonnelContacts
                {
                    Phone = createDto.Contacts.Phone,
                    Email = createDto.Contacts.Email,
                    Address = createDto.Contacts.Address
                },
                Documents = createDto.Documents?.Select(d => new PersonnelDocumentInfo
                {
                    Type = d.Type,
                    Number = d.Number,
                    IssuedBy = d.IssuedBy,
                    IssuedOn = d.IssuedOn,
                    Category = d.Category,
                    ValidUntil = d.ValidUntil
                }).ToList() ?? new List<PersonnelDocumentInfo>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _personnelRepository.CreateAsync(personnel);
            return MapToDto(created);
        }

        public async Task<bool> UpdatePersonnelAsync(string id, UpdatePersonnelDto updateDto)
        {
            var existing = await _personnelRepository.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.FullName = updateDto.FullName;
            existing.BirthDate = updateDto.BirthDate;
            existing.Position = updateDto.Position;
            existing.Status = updateDto.Status;
            existing.Contacts = new PersonnelContacts
            {
                Phone = updateDto.Contacts.Phone,
                Email = updateDto.Contacts.Email,
                Address = updateDto.Contacts.Address
            };
            existing.UpdatedAt = DateTime.UtcNow;

            return await _personnelRepository.UpdateAsync(id, existing);
        }

        public async Task<bool> DeletePersonnelAsync(string id)
        {
            return await _personnelRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdatePersonnelStatusAsync(string id, string status)
        {
            return await _personnelRepository.UpdateStatusAsync(id, status);
        }

        public async Task<bool> UpdatePersonnelContactsAsync(string id, PersonnelContactsDto contactsDto)
        {
            var contacts = new PersonnelContacts
            {
                Phone = contactsDto.Phone,
                Email = contactsDto.Email,
                Address = contactsDto.Address
            };

            return await _personnelRepository.UpdateContactsAsync(id, contacts);
        }

        public async Task<bool> AddPersonnelDocumentAsync(string id, PersonnelDocumentInfoDto documentDto)
        {
            var document = new PersonnelDocumentInfo
            {
                Type = documentDto.Type,
                Number = documentDto.Number,
                IssuedBy = documentDto.IssuedBy,
                IssuedOn = documentDto.IssuedOn,
                Category = documentDto.Category,
                ValidUntil = documentDto.ValidUntil
            };

            return await _personnelRepository.AddDocumentAsync(id, document);
        }

        private PersonnelDto MapToDto(Personnel personnel)
        {
            return new PersonnelDto
            {
                Id = personnel.Id,
                PersonnelId = personnel.PersonnelId,
                FullName = personnel.FullName,
                BirthDate = personnel.BirthDate,
                Position = personnel.Position,
                Status = personnel.Status,
                Contacts = new PersonnelContactsDto
                {
                    Phone = personnel.Contacts.Phone,
                    Email = personnel.Contacts.Email,
                    Address = personnel.Contacts.Address
                },
                Documents = personnel.Documents.Select(d => new PersonnelDocumentInfoDto
                {
                    Type = d.Type,
                    Number = d.Number,
                    IssuedBy = d.IssuedBy,
                    IssuedOn = d.IssuedOn,
                    Category = d.Category,
                    ValidUntil = d.ValidUntil
                }).ToList()
            };
        }
    }
}