using Dapper;
using TechnicalService.Domain.Entities;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations.Dapper;

public class RepairPartRepository : IRepairPartRepository
{
    private readonly DapperContext _context;

    public RepairPartRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RepairPart>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT PartId, PartName, PartNumber, UnitPrice, StockQuantity, 
                   Supplier, CreatedAt, UpdatedAt, IsDeleted
            FROM RepairPart 
            WHERE IsDeleted = @IsDeleted 
            ORDER BY PartName";

        var command = new CommandDefinition(
            sql,
            new { IsDeleted = false },
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<RepairPart>(command);
    }

    public async Task<RepairPart?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT PartId, PartName, PartNumber, UnitPrice, StockQuantity, 
                   Supplier, CreatedAt, UpdatedAt, IsDeleted
            FROM RepairPart 
            WHERE PartId = @PartId AND IsDeleted = @IsDeleted";

        var command = new CommandDefinition(
            sql,
            new { PartId = id, IsDeleted = false },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<RepairPart>(command);
    }

    public async Task<int> AddAsync(
        RepairPart entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            INSERT INTO RepairPart 
            (PartName, PartNumber, UnitPrice, StockQuantity, Supplier)
            VALUES 
            (@PartName, @PartNumber, @UnitPrice, @StockQuantity, @Supplier);
            SELECT LAST_INSERT_ID();";

        var command = new CommandDefinition(
            sql,
            entity,
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<int> UpdateAsync(
        RepairPart entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE RepairPart 
            SET PartName = @PartName,
                PartNumber = @PartNumber,
                UnitPrice = @UnitPrice,
                StockQuantity = @StockQuantity,
                Supplier = @Supplier,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE PartId = @PartId AND IsDeleted = @IsDeleted";

        var command = new CommandDefinition(
            sql,
            new
            {
                entity.PartId,
                entity.PartName,
                entity.PartNumber,
                entity.UnitPrice,
                entity.StockQuantity,
                entity.Supplier,
                IsDeleted = false
            },
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command);
    }

    public async Task<int> DeleteAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE RepairPart 
            SET IsDeleted = @IsDeleted,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE PartId = @PartId";

        var command = new CommandDefinition(
            sql,
            new { PartId = id, IsDeleted = true },
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command);
    }

    // IRepairPartRepository специфічні методи 

    /// Отримати запчастини з низьким запасом на складі
    public async Task<IEnumerable<RepairPart>> GetLowStockPartsAsync(
        int threshold,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT PartId, PartName, PartNumber, UnitPrice, StockQuantity, 
                   Supplier, CreatedAt, UpdatedAt, IsDeleted
            FROM RepairPart 
            WHERE StockQuantity <= @Threshold 
              AND IsDeleted = @IsDeleted
            ORDER BY StockQuantity ASC, PartName";

        var command = new CommandDefinition(
            sql,
            new { Threshold = threshold, IsDeleted = false },
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<RepairPart>(command);
    }

    /// Оновити кількість запчастин на складі (збільшити/зменшити)
    /// Використовується при постачанні або використанні запчастин
    public async Task<int> UpdateStockQuantityAsync(
        int partId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE RepairPart 
            SET StockQuantity = StockQuantity + @Quantity,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE PartId = @PartId 
              AND IsDeleted = @IsDeleted
              AND (StockQuantity + @Quantity) >= 0";  // Запобігаємо від'ємній кількості

        var command = new CommandDefinition(
            sql,
            new { PartId = partId, Quantity = quantity, IsDeleted = false },
            cancellationToken: cancellationToken);

        var affectedRows = await connection.ExecuteAsync(command);

        // Якщо 0 рядків оновлено - можливо спроба зробити від'ємну кількість
        if (affectedRows == 0 && quantity < 0)
        {
            // Перевіримо, чи існує запчастина
            var part = await GetByIdAsync(partId, cancellationToken);
            if (part != null && part.StockQuantity + quantity < 0)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for part {partId}. " +
                    $"Available: {part.StockQuantity}, Requested: {Math.Abs(quantity)}");
            }
        }

        return affectedRows;
    }

    /// Знайти запчастину за артикулом
    public async Task<RepairPart?> GetByPartNumberAsync(
        string partNumber,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT PartId, PartName, PartNumber, UnitPrice, StockQuantity, 
                   Supplier, CreatedAt, UpdatedAt, IsDeleted
            FROM RepairPart 
            WHERE PartNumber = @PartNumber 
              AND IsDeleted = @IsDeleted";

        var command = new CommandDefinition(
            sql,
            new { PartNumber = partNumber, IsDeleted = false },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<RepairPart>(command);
    }
}