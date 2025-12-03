using PersonnelService.Domain.Entities;

namespace PersonnelService.Domain.Interfaces
{
    public interface IPersonnelRepository
    {
        Task<Personnel?> GetByIdAsync(string id);
        Task<Personnel?> GetByPersonnelIdAsync(int personnelId);
        Task<IReadOnlyCollection<Personnel>> GetAllAsync();
        Task<IReadOnlyCollection<Personnel>> GetByPositionAsync(string position);
        Task<IReadOnlyCollection<Personnel>> GetByStatusAsync(string status);
        Task<IReadOnlyCollection<Personnel>> GetActivePersonnelAsync();

        Task AddAsync(Personnel personnel);
        Task UpdateAsync(Personnel personnel);
        Task DeleteAsync(string id);

        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByPersonnelIdAsync(int personnelId, CancellationToken cancellationToken = default);
        Task<int> GetNextPersonnelIdAsync();

        // MongoDB specific - aggregation, pagination
        Task<IReadOnlyCollection<Personnel>> SearchAsync(
            string? searchText = null,
            string? position = null,
            string? status = null,
            int skip = 0,
            int limit = 10);
    }
}