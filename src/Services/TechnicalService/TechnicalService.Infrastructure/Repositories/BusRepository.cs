using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using TechnicalService.Core.Entities;
using TechnicalService.Core.Interfaces;
using TechnicalService.Infrastructure.Data;

namespace TechnicalService.Infrastructure.Repositories;

public class BusRepository : IBusRepository
{
    private readonly DapperContext _context;

    public BusRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bus>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT b.*, bs.StatusId, bs.StatusName, bs.StatusDescription, 
                   bs.CreatedAt AS Status_CreatedAt, bs.UpdatedAt AS Status_UpdatedAt
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.IsDeleted = FALSE
            ORDER BY b.CountryNumber";

        var busDictionary = new Dictionary<string, Bus>();

        var result = await connection.QueryAsync<Bus, BusStatus, Bus>(
            sql,
            (bus, status) =>
            {
                if (!busDictionary.TryGetValue(bus.CountryNumber, out var busEntry))
                {
                    busEntry = bus;
                    busEntry.CurrentStatus = status;
                    busDictionary.Add(bus.CountryNumber, busEntry);
                }
                return busEntry;
            },
            splitOn: "StatusId"
        );

        return busDictionary.Values;
    }

    public async Task<Bus?> GetByIdAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM Bus 
            WHERE CountryNumber = @CountryNumber AND IsDeleted = FALSE";

        return await connection.QueryFirstOrDefaultAsync<Bus>(sql, new { CountryNumber = id });
    }

    public async Task<Bus?> GetBusWithStatusAsync(string countryNumber)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT b.*, bs.*
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.CountryNumber = @CountryNumber AND b.IsDeleted = FALSE";

        var result = await connection.QueryAsync<Bus, BusStatus, Bus>(
            sql,
            (bus, status) =>
            {
                bus.CurrentStatus = status;
                return bus;
            },
            new { CountryNumber = countryNumber },
            splitOn: "StatusId"
        );

        return result.FirstOrDefault();
    }

    public async Task<int> AddAsync(Bus entity)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            INSERT INTO Bus (CountryNumber, BoardingNumber, Brand, PassengerCapacity, 
                           YearOfManufacture, Mileage, DateOfReceipt, CurrentStatusId)
            VALUES (@CountryNumber, @BoardingNumber, @Brand, @PassengerCapacity, 
                   @YearOfManufacture, @Mileage, @DateOfReceipt, @CurrentStatusId)";

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> UpdateAsync(Bus entity)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE Bus 
            SET BoardingNumber = @BoardingNumber,
                Brand = @Brand,
                PassengerCapacity = @PassengerCapacity,
                YearOfManufacture = @YearOfManufacture,
                Mileage = @Mileage,
                CurrentStatusId = @CurrentStatusId,
                WriteoffDate = @WriteoffDate
            WHERE CountryNumber = @CountryNumber AND IsDeleted = FALSE";

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE Bus 
            SET IsDeleted = TRUE 
            WHERE CountryNumber = @CountryNumber";

        return await connection.ExecuteAsync(sql, new { CountryNumber = id });
    }

    public async Task<IEnumerable<Bus>> GetBusesByStatusAsync(int statusId)
    {
        using var connection = _context.CreateConnection();

        var parameters = new { StatusId = statusId };
        return await connection.QueryAsync<Bus>(
            "sp_GetBusesByStatus",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<Bus>> GetActiveBusesAsync()
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT b.*, bs.*
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.IsDeleted = FALSE 
              AND b.WriteoffDate IS NULL
              AND bs.StatusName = 'Active'";

        var busDictionary = new Dictionary<string, Bus>();

        await connection.QueryAsync<Bus, BusStatus, Bus>(
            sql,
            (bus, status) =>
            {
                if (!busDictionary.TryGetValue(bus.CountryNumber, out var busEntry))
                {
                    busEntry = bus;
                    busEntry.CurrentStatus = status;
                    busDictionary.Add(bus.CountryNumber, busEntry);
                }
                return busEntry;
            },
            splitOn: "StatusId"
        );

        return busDictionary.Values;
    }

    public async Task<int> UpdateBusStatusAsync(string countryNumber, int newStatusId)
    {
        using var connection = _context.CreateConnection();

        var parameters = new
        {
            p_CountryNumber = countryNumber,
            p_NewStatusId = newStatusId
        };

        return await connection.ExecuteAsync(
            "sp_UpdateBusStatus",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<decimal> GetTotalMileageAsync(string countryNumber)
    {
        using var connection = _context.CreateConnection();

        var sql = "SELECT Mileage FROM Bus WHERE CountryNumber = @CountryNumber";

        return await connection.QueryFirstOrDefaultAsync<decimal>(sql, new { CountryNumber = countryNumber });
    }
}