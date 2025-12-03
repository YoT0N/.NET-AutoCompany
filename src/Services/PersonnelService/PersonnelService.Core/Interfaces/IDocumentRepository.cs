using PersonnelService.Domain.Entities;

namespace PersonnelService.Domain.Interfaces
{
    public interface IDocumentRepository
    {
        Task<PersonnelDocument?> GetByIdAsync(string id);
        Task<IReadOnlyCollection<PersonnelDocument>> GetAllAsync();
        Task<IReadOnlyCollection<PersonnelDocument>> GetByPersonnelIdAsync(int personnelId);
        Task<IReadOnlyCollection<PersonnelDocument>> GetByDocTypeAsync(string docType);
        Task<IReadOnlyCollection<PersonnelDocument>> GetExpiredDocumentsAsync(DateTime? beforeDate = null);
        Task<IReadOnlyCollection<PersonnelDocument>> GetExpiringDocumentsAsync(int daysThreshold = 30);
        Task<IReadOnlyCollection<PersonnelDocument>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task AddAsync(PersonnelDocument document);
        Task UpdateAsync(PersonnelDocument document);
        Task DeleteAsync(string id);
        Task DeleteByPersonnelIdAsync(int personnelId);

        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> HasDocumentTypeAsync(int personnelId, string docType, CancellationToken cancellationToken = default);

        // Aggregation methods
        Task<Dictionary<string, int>> GetDocumentCountByTypeAsync();
        Task<IReadOnlyCollection<PersonnelDocument>> SearchAsync(
            int? personnelId = null,
            string? docType = null,
            bool? onlyExpired = null,
            bool? onlyExpiring = null,
            int skip = 0,
            int limit = 10);
    }
}