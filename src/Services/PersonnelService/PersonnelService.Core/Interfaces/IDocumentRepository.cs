using PersonnelService.Core.Models;

namespace PersonnelService.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<PersonnelDocument>> GetAllAsync();
        Task<PersonnelDocument?> GetByIdAsync(string id);
        Task<IEnumerable<PersonnelDocument>> GetByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<PersonnelDocument>> GetByDocTypeAsync(string docType);
        Task<IEnumerable<PersonnelDocument>> GetExpiredDocumentsAsync(DateTime beforeDate);
        Task<IEnumerable<PersonnelDocument>> GetExpiringDocumentsAsync(DateTime withinDate);
        Task<PersonnelDocument> CreateAsync(PersonnelDocument document);
        Task<bool> UpdateAsync(string id, PersonnelDocument document);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByPersonnelIdAsync(int personnelId);
    }
}