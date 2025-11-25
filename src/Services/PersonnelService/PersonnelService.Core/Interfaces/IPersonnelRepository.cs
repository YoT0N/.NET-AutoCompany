using PersonnelService.Core.Models;

namespace PersonnelService.Core.Interfaces
{
    public interface IPersonnelRepository
    {
        Task<IEnumerable<Personnel>> GetAllAsync();
        Task<Personnel?> GetByIdAsync(string id);
        Task<Personnel?> GetByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<Personnel>> GetByPositionAsync(string position);
        Task<IEnumerable<Personnel>> GetByStatusAsync(string status);
        Task<IEnumerable<Personnel>> GetActivePersonnelAsync();
        Task<Personnel> CreateAsync(Personnel personnel);
        Task<bool> UpdateAsync(string id, Personnel personnel);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateStatusAsync(string id, string status);
        Task<bool> UpdateContactsAsync(string id, PersonnelContacts contacts);
        Task<bool> AddDocumentAsync(string id, PersonnelDocumentInfo document);
        Task<int> GetNextPersonnelIdAsync();
    }
}