using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IRepairPartRepository : IRepository<RepairPart>
{
    Task<IEnumerable<RepairPart>> GetLowStockPartsAsync(int threshold);
    Task<int> UpdateStockQuantityAsync(int partId, int quantity);
    Task<RepairPart?> GetByPartNumberAsync(string partNumber);
}