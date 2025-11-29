using Dapper;
using System.Data;
using TechnicalService.Domain.Entities;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations.Dapper;

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly DapperContext _context;

    public MaintenanceRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BusMaintenanceHistory>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM BusMaintenanceHistory 
            ORDER BY MaintenanceDate DESC";

        return await connection.QueryAsync<BusMaintenanceHistory>(sql);
    }

    public async Task<BusMaintenanceHistory?> GetByIdAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = "SELECT * FROM BusMaintenanceHistory WHERE MaintenanceId = @MaintenanceId";

        return await connection.QueryFirstOrDefaultAsync<BusMaintenanceHistory>(
            sql,
            new { MaintenanceId = id }
        );
    }

    public async Task<int> AddAsync(BusMaintenanceHistory entity)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            INSERT INTO BusMaintenanceHistory 
            (BusCountryNumber, MaintenanceDate, MaintenanceType, Description, 
             Cost, MechanicName, NextMaintenanceDate)
            VALUES 
            (@BusCountryNumber, @MaintenanceDate, @MaintenanceType, @Description, 
             @Cost, @MechanicName, @NextMaintenanceDate)";

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> UpdateAsync(BusMaintenanceHistory entity)
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

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = "DELETE FROM BusMaintenanceHistory WHERE MaintenanceId = @MaintenanceId";

        return await connection.ExecuteAsync(sql, new { MaintenanceId = id });
    }

    public async Task<IEnumerable<BusMaintenanceHistory>> GetMaintenanceByBusAsync(string countryNumber)
    {
        using var connection = _context.CreateConnection();

        var parameters = new { p_BusCountryNumber = countryNumber };

        return await connection.QueryAsync<BusMaintenanceHistory>(
            "sp_GetBusMaintenanceHistory",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<decimal> GetTotalMaintenanceCostAsync(string countryNumber)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("p_BusCountryNumber", countryNumber);
        parameters.Add("p_TotalCost", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_CalculateMaintenanceCost",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return parameters.Get<decimal>("p_TotalCost");
    }

    public async Task<IEnumerable<BusMaintenanceHistory>> GetUpcomingMaintenanceAsync(DateTime fromDate)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM BusMaintenanceHistory 
            WHERE NextMaintenanceDate IS NOT NULL 
              AND NextMaintenanceDate >= @FromDate
            ORDER BY NextMaintenanceDate";

        return await connection.QueryAsync<BusMaintenanceHistory>(
            sql,
            new { FromDate = fromDate }
        );
    }
}