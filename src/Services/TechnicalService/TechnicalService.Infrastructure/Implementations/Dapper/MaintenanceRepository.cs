using Dapper;
using System.Data;
using TechnicalService.Domain.Entities;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations.Dapper;

/// <summary>
/// Репозиторій історії обслуговування автобусів (ADO.NET + Dapper)
/// Демонструє роботу з Dapper для читань, multi-mapping та збереженими процедурами
/// </summary>
public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly DapperContext _context;

    public MaintenanceRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BusMaintenanceHistory>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT m.MaintenanceId, m.BusCountryNumber, m.MaintenanceDate, 
                   m.MaintenanceType, m.Description, m.Cost, m.MechanicName, 
                   m.NextMaintenanceDate, m.CreatedAt, m.CreatedBy,
                   b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt
            FROM BusMaintenanceHistory m
            LEFT JOIN Bus b ON m.BusCountryNumber = b.CountryNumber
            ORDER BY m.MaintenanceDate DESC";

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        // Multi-mapping: BusMaintenanceHistory + Bus
        var result = await connection.QueryAsync<BusMaintenanceHistory, Bus, BusMaintenanceHistory>(
            command,
            (maintenance, bus) =>
            {
                maintenance.Bus = bus;
                return maintenance;
            },
            splitOn: "CountryNumber"
        );

        return result;
    }

    public async Task<BusMaintenanceHistory?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT m.MaintenanceId, m.BusCountryNumber, m.MaintenanceDate, 
                   m.MaintenanceType, m.Description, m.Cost, m.MechanicName, 
                   m.NextMaintenanceDate, m.CreatedAt, m.CreatedBy,
                   b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt
            FROM BusMaintenanceHistory m
            LEFT JOIN Bus b ON m.BusCountryNumber = b.CountryNumber
            WHERE m.MaintenanceId = @MaintenanceId";

        var command = new CommandDefinition(
            sql,
            new { MaintenanceId = id },
            cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<BusMaintenanceHistory, Bus, BusMaintenanceHistory>(
            command,
            (maintenance, bus) =>
            {
                maintenance.Bus = bus;
                return maintenance;
            },
            splitOn: "CountryNumber"
        );

        return result.FirstOrDefault();
    }

    public async Task<int> AddAsync(
        BusMaintenanceHistory entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            INSERT INTO BusMaintenanceHistory 
            (BusCountryNumber, MaintenanceDate, MaintenanceType, Description, 
             Cost, MechanicName, NextMaintenanceDate, CreatedBy)
            VALUES 
            (@BusCountryNumber, @MaintenanceDate, @MaintenanceType, @Description, 
             @Cost, @MechanicName, @NextMaintenanceDate, @CreatedBy);
            SELECT LAST_INSERT_ID();";

        var command = new CommandDefinition(
            sql,
            entity,
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<int> UpdateAsync(
        BusMaintenanceHistory entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE BusMaintenanceHistory 
            SET MaintenanceDate = @MaintenanceDate,
                MaintenanceType = @MaintenanceType,
                Description = @Description,
                Cost = @Cost,
                MechanicName = @MechanicName,
                NextMaintenanceDate = @NextMaintenanceDate
            WHERE MaintenanceId = @MaintenanceId";

        var command = new CommandDefinition(
            sql,
            entity,
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command);
    }

    public async Task<int> DeleteAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = "DELETE FROM BusMaintenanceHistory WHERE MaintenanceId = @MaintenanceId";

        var command = new CommandDefinition(
            sql,
            new { MaintenanceId = id },
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command);
    }

    // ========== IMaintenanceRepository специфічні методи ==========

    /// <summary>
    /// Отримати історію обслуговування для конкретного автобуса
    /// Використовує збережену процедуру
    /// </summary>
    public async Task<IEnumerable<BusMaintenanceHistory>> GetMaintenanceByBusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var parameters = new { p_BusCountryNumber = countryNumber };

        var command = new CommandDefinition(
            "sp_GetBusMaintenanceHistory",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<BusMaintenanceHistory>(command);
    }

    /// <summary>
    /// Розрахувати загальну вартість обслуговування для автобуса
    /// Використовує збережену процедуру з OUT параметром
    /// </summary>
    public async Task<decimal> GetTotalMaintenanceCostAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("p_BusCountryNumber", countryNumber);
        parameters.Add("p_TotalCost", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 10, scale: 2);

        var command = new CommandDefinition(
            "sp_CalculateMaintenanceCost",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<decimal>("p_TotalCost");
    }

    /// <summary>
    /// Отримати майбутні заплановані обслуговування
    /// </summary>
    public async Task<IEnumerable<BusMaintenanceHistory>> GetUpcomingMaintenanceAsync(
        DateTime fromDate,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT m.MaintenanceId, m.BusCountryNumber, m.MaintenanceDate, 
                   m.MaintenanceType, m.Description, m.Cost, m.MechanicName, 
                   m.NextMaintenanceDate, m.CreatedAt, m.CreatedBy,
                   b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt
            FROM BusMaintenanceHistory m
            LEFT JOIN Bus b ON m.BusCountryNumber = b.CountryNumber
            WHERE m.NextMaintenanceDate IS NOT NULL 
              AND m.NextMaintenanceDate >= @FromDate
            ORDER BY m.NextMaintenanceDate";

        var command = new CommandDefinition(
            sql,
            new { FromDate = fromDate },
            cancellationToken: cancellationToken);

        // Multi-mapping з Bus
        var result = await connection.QueryAsync<BusMaintenanceHistory, Bus, BusMaintenanceHistory>(
            command,
            (maintenance, bus) =>
            {
                maintenance.Bus = bus;
                return maintenance;
            },
            splitOn: "CountryNumber"
        );

        return result;
    }
}