using PersonnelService.Application.Interfaces;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;

namespace PersonnelService.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<IEnumerable<PersonnelDocument>> GetAllDocumentsAsync()
        {
            return await _documentRepository.GetAllAsync();
        }

        public async Task<PersonnelDocument?> GetDocumentByIdAsync(string id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PersonnelDocument>> GetDocumentsByPersonnelIdAsync(int personnelId)
        {
            return await _documentRepository.GetByPersonnelIdAsync(personnelId);
        }

        public async Task<IEnumerable<PersonnelDocument>> GetDocumentsByTypeAsync(string docType)
        {
            return await _documentRepository.GetByDocTypeAsync(docType);
        }

        public async Task<IEnumerable<PersonnelDocument>> GetExpiredDocumentsAsync(DateTime beforeDate)
        {
            return await _documentRepository.GetExpiredDocumentsAsync(beforeDate);
        }

        public async Task<IEnumerable<PersonnelDocument>> GetExpiringDocumentsAsync(DateTime withinDate)
        {
            return await _documentRepository.GetExpiringDocumentsAsync(withinDate);
        }

        public async Task<PersonnelDocument> CreateDocumentAsync(PersonnelDocument document)
        {
            document.UploadedAt = DateTime.UtcNow;
            return await _documentRepository.CreateAsync(document);
        }

        public async Task<bool> UpdateDocumentAsync(string id, PersonnelDocument document)
        {
            return await _documentRepository.UpdateAsync(id, document);
        }

        public async Task<bool> DeleteDocumentAsync(string id)
        {
            return await _documentRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteDocumentsByPersonnelIdAsync(int personnelId)
        {
            return await _documentRepository.DeleteByPersonnelIdAsync(personnelId);
        }
    }
}