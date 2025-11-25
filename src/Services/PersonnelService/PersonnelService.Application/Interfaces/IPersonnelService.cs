using PersonnelService.Core.DTOs;
using PersonnelService.Core.Models;

namespace PersonnelService.Application.Interfaces
{
    public interface IPersonnelService
    {
        Task<IEnumerable<PersonnelDto>> GetAllPersonnelAsync();
        Task<PersonnelDto?> GetPersonnelByIdAsync(string id);
        Task<PersonnelDto?> GetPersonnelByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<PersonnelDto>> GetPersonnelByPositionAsync(string position);
        Task<IEnumerable<PersonnelDto>> GetPersonnelByStatusAsync(string status);
        Task<IEnumerable<PersonnelDto>> GetActivePersonnelAsync();
        Task<PersonnelDto> CreatePersonnelAsync(CreatePersonnelDto createDto);
        Task<bool> UpdatePersonnelAsync(string id, UpdatePersonnelDto updateDto);
        Task<bool> DeletePersonnelAsync(string id);
        Task<bool> UpdatePersonnelStatusAsync(string id, string status);
        Task<bool> UpdatePersonnelContactsAsync(string id, PersonnelContactsDto contactsDto);
        Task<bool> AddPersonnelDocumentAsync(string id, PersonnelDocumentInfoDto documentDto);
    }
}