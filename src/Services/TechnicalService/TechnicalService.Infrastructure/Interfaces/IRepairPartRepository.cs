using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IRepairPartRepository : IRepository<RepairPart>
{
    Task<IEnumerable<RepairPart>> GetLowStockPartsAsync(int threshold, CancellationToken cancellationToken = default);
    Task<int> UpdateStockQuantityAsync(int partId, int quantity, CancellationToken cancellationToken = default);
    Task<RepairPart?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default);
}