using Dapper;
using TechnicalService.Domain.Entities;
using TechnicalService.Core.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations.Dapper;

public class RepairPartRepository : IRepairPartRepository
{
    private readonly DapperContext _context;

    public RepairPartRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RepairPart>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        var sql = "SELECT * FROM RepairPart WHERE IsDeleted = FALSE ORDER BY PartName";

        return await connection.QueryAsync<RepairPart>(sql);
    }

    public async Task<RepairPart?> GetByIdAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = "SELECT * FROM RepairPart WHERE PartId = @PartId AND IsDeleted = FALSE";

        return await connection.QueryFirstOrDefaultAsync<RepairPart>(sql, new { PartId = id });
    }

    public async Task<int> AddAsync(RepairPart entity)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            INSERT INTO RepairPart (PartName, PartNumber, UnitPrice, StockQuantity, Supplier)
            VALUES (@PartName, @PartNumber, @UnitPrice, @StockQuantity, @Supplier)";

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> UpdateAsync(RepairPart entity)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE RepairPart 
            SET PartName = @PartName,
                PartNumber = @PartNumber,
                UnitPrice = @UnitPrice,
                StockQuantity = @StockQuantity,
                Supplier = @Supplier
            WHERE PartId = @PartId AND IsDeleted = FALSE";

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = "UPDATE RepairPart SET IsDeleted = TRUE WHERE PartId = @PartId";

        return await connection.ExecuteAsync(sql, new { PartId = id });
    }

    public async Task<IEnumerable<RepairPart>> GetLowStockPartsAsync(int threshold)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM RepairPart 
            WHERE StockQuantity <= @Threshold AND IsDeleted = FALSE
            ORDER BY StockQuantity";

        return await connection.QueryAsync<RepairPart>(sql, new { Threshold = threshold });
    }

    public async Task<int> UpdateStockQuantityAsync(int partId, int quantity)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE RepairPart 
            SET StockQuantity = StockQuantity + @Quantity
            WHERE PartId = @PartId AND IsDeleted = FALSE";

        return await connection.ExecuteAsync(sql, new { PartId = partId, Quantity = quantity });
    }

    public async Task<RepairPart?> GetByPartNumberAsync(string partNumber)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM RepairPart 
            WHERE PartNumber = @PartNumber AND IsDeleted = FALSE";

        return await connection.QueryFirstOrDefaultAsync<RepairPart>(sql, new { PartNumber = partNumber });
    }
}