using TechnicalService.Application.Interfaces;
using TechnicalService.Core.Entities;
using TechnicalService.Core.Interfaces;

namespace TechnicalService.Application.Services;

public class RepairPartService : IRepairPartService
{
    private readonly IUnitOfWork _unitOfWork;

    public RepairPartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RepairPart>> GetAllPartsAsync()
    {
        return await _unitOfWork.RepairParts.GetAllAsync();
    }

    public async Task<RepairPart?> GetPartByIdAsync(int partId)
    {
        return await _unitOfWork.RepairParts.GetByIdAsync(partId);
    }

    public async Task<RepairPart?> GetPartByPartNumberAsync(string partNumber)
    {
        return await _unitOfWork.RepairParts.GetByPartNumberAsync(partNumber);
    }

    public async Task<IEnumerable<RepairPart>> GetLowStockPartsAsync(int threshold)
    {
        return await _unitOfWork.RepairParts.GetLowStockPartsAsync(threshold);
    }

    public async Task<int> CreatePartAsync(RepairPart part)
    {
        return await _unitOfWork.RepairParts.AddAsync(part);
    }

    public async Task<int> UpdatePartAsync(RepairPart part)
    {
        return await _unitOfWork.RepairParts.UpdateAsync(part);
    }

    public async Task<int> DeletePartAsync(int partId)
    {
        return await _unitOfWork.RepairParts.DeleteAsync(partId);
    }

    public async Task<int> UpdateStockQuantityAsync(int partId, int quantity)
    {
        return await _unitOfWork.RepairParts.UpdateStockQuantityAsync(partId, quantity);
    }
}