using PersonnelService.Core.Models;

namespace PersonnelService.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<PersonnelDocument>> GetAllDocumentsAsync();
        Task<PersonnelDocument?> GetDocumentByIdAsync(string id);
        Task<IEnumerable<PersonnelDocument>> GetDocumentsByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<PersonnelDocument>> GetDocumentsByTypeAsync(string docType);
        Task<IEnumerable<PersonnelDocument>> GetExpiredDocumentsAsync(DateTime beforeDate);
        Task<IEnumerable<PersonnelDocument>> GetExpiringDocumentsAsync(DateTime withinDate);
        Task<PersonnelDocument> CreateDocumentAsync(PersonnelDocument document);
        Task<bool> UpdateDocumentAsync(string id, PersonnelDocument document);
        Task<bool> DeleteDocumentAsync(string id);
        Task<bool> DeleteDocumentsByPersonnelIdAsync(int personnelId);
    }
}