using TechnicalService.Core.Entities;

namespace TechnicalService.Application.Interfaces;

public interface IRepairPartService
{
    Task<IEnumerable<RepairPart>> GetAllPartsAsync();
    Task<RepairPart?> GetPartByIdAsync(int partId);
    Task<RepairPart?> GetPartByPartNumberAsync(string partNumber);
    Task<IEnumerable<RepairPart>> GetLowStockPartsAsync(int threshold);
    Task<int> CreatePartAsync(RepairPart part);
    Task<int> UpdatePartAsync(RepairPart part);
    Task<int> DeletePartAsync(int partId);
    Task<int> UpdateStockQuantityAsync(int partId, int quantity);
}