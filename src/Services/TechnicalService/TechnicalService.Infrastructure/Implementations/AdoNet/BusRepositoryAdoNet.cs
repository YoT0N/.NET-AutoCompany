using System.Data;
using MySqlConnector;
using TechnicalService.Domain.Entities;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations.AdoNet;

public class BusRepositoryAdoNet : IBusRepository
{
    private readonly DapperContext _context;

    public BusRepositoryAdoNet(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bus>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var buses = new List<Bus>();

        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            SELECT b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt,
                   bs.StatusId, bs.StatusName, bs.StatusDescription
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.IsDeleted = @IsDeleted
            ORDER BY b.CountryNumber";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@IsDeleted", false);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            buses.Add(MapBusFromReader(reader));
        }

        return buses;
    }

    public async Task<Bus?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            SELECT b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt,
                   bs.StatusId, bs.StatusName, bs.StatusDescription
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.CountryNumber = @CountryNumber AND b.IsDeleted = @IsDeleted";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@CountryNumber", id);
        command.Parameters.AddWithValue("@IsDeleted", false);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapBusFromReader(reader) : null;
    }

    public async Task<int> AddAsync(Bus entity, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            INSERT INTO Bus (CountryNumber, BoardingNumber, Brand, PassengerCapacity,
                           YearOfManufacture, Mileage, DateOfReceipt, CurrentStatusId)
            VALUES (@CountryNumber, @BoardingNumber, @Brand, @PassengerCapacity,
                   @YearOfManufacture, @Mileage, @DateOfReceipt, @CurrentStatusId)";

        await using var command = new MySqlCommand(commandText, connection);

        command.Parameters.AddWithValue("@CountryNumber", entity.CountryNumber);
        command.Parameters.AddWithValue("@BoardingNumber", entity.BoardingNumber);
        command.Parameters.AddWithValue("@Brand", entity.Brand);
        command.Parameters.AddWithValue("@PassengerCapacity", entity.PassengerCapacity);
        command.Parameters.AddWithValue("@YearOfManufacture", entity.YearOfManufacture);
        command.Parameters.AddWithValue("@Mileage", entity.Mileage);
        command.Parameters.AddWithValue("@DateOfReceipt", entity.DateOfReceipt);
        command.Parameters.AddWithValue("@CurrentStatusId", entity.CurrentStatusId);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(Bus entity, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            UPDATE Bus 
            SET BoardingNumber = @BoardingNumber,
                Brand = @Brand,
                PassengerCapacity = @PassengerCapacity,
                Mileage = @Mileage,
                CurrentStatusId = @CurrentStatusId,
                WriteoffDate = @WriteoffDate,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE CountryNumber = @CountryNumber AND IsDeleted = @IsDeleted";

        await using var command = new MySqlCommand(commandText, connection);

        command.Parameters.AddWithValue("@CountryNumber", entity.CountryNumber);
        command.Parameters.AddWithValue("@BoardingNumber", entity.BoardingNumber);
        command.Parameters.AddWithValue("@Brand", entity.Brand);
        command.Parameters.AddWithValue("@PassengerCapacity", entity.PassengerCapacity);
        command.Parameters.AddWithValue("@Mileage", entity.Mileage);
        command.Parameters.AddWithValue("@CurrentStatusId", entity.CurrentStatusId);
        command.Parameters.AddWithValue("@WriteoffDate", entity.WriteoffDate.HasValue
            ? entity.WriteoffDate.Value
            : DBNull.Value);
        command.Parameters.AddWithValue("@IsDeleted", false);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(object id, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            UPDATE Bus 
            SET IsDeleted = @IsDeleted,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE CountryNumber = @CountryNumber";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@IsDeleted", true);
        command.Parameters.AddWithValue("@CountryNumber", id);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    // ========== IBusRepository специфічні методи ==========

    public async Task<Bus?> GetBusWithStatusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            SELECT b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt,
                   bs.StatusId, bs.StatusName, bs.StatusDescription
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.CountryNumber = @CountryNumber AND b.IsDeleted = @IsDeleted";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@CountryNumber", countryNumber);
        command.Parameters.AddWithValue("@IsDeleted", false);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? MapBusFromReader(reader) : null;
    }

    public async Task<IEnumerable<Bus>> GetBusesByStatusAsync(
        int statusId,
        CancellationToken cancellationToken = default)
    {
        var buses = new List<Bus>();

        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        // Використання збереженої процедури
        await using var command = new MySqlCommand("sp_GetBusesByStatus", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@StatusId", statusId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            buses.Add(MapBusFromReader(reader));
        }

        return buses;
    }

    public async Task<IEnumerable<Bus>> GetActiveBusesAsync(
        CancellationToken cancellationToken = default)
    {
        var buses = new List<Bus>();

        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            SELECT b.CountryNumber, b.BoardingNumber, b.Brand, b.PassengerCapacity,
                   b.YearOfManufacture, b.Mileage, b.DateOfReceipt, b.WriteoffDate,
                   b.CurrentStatusId, b.IsDeleted, b.CreatedAt, b.UpdatedAt,
                   bs.StatusId, bs.StatusName, bs.StatusDescription
            FROM Bus b
            INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
            WHERE b.IsDeleted = @IsDeleted 
              AND b.WriteoffDate IS NULL
              AND bs.StatusName = @StatusName";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@IsDeleted", false);
        command.Parameters.AddWithValue("@StatusName", "Active");

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            buses.Add(MapBusFromReader(reader));
        }

        return buses;
    }

    public async Task<int> UpdateBusStatusAsync(
        string countryNumber,
        int newStatusId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        // Використання збереженої процедури
        await using var command = new MySqlCommand("sp_UpdateBusStatus", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@p_CountryNumber", countryNumber);
        command.Parameters.AddWithValue("@p_NewStatusId", newStatusId);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalMileageAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateConnection() as MySqlConnection;
        if (connection == null) throw new InvalidOperationException("Connection is not MySqlConnection");

        await connection.OpenAsync(cancellationToken);

        const string commandText = @"
            SELECT Mileage 
            FROM Bus 
            WHERE CountryNumber = @CountryNumber AND IsDeleted = @IsDeleted";

        await using var command = new MySqlCommand(commandText, connection);
        command.Parameters.AddWithValue("@CountryNumber", countryNumber);
        command.Parameters.AddWithValue("@IsDeleted", false);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null and not DBNull ? Convert.ToDecimal(result) : 0;
    }

    // ========== HELPER METHODS ==========

    private static Bus MapBusFromReader(IDataRecord reader)
    {
        // Локальні функції для безпечного читання
        static bool HasColumn(IDataRecord r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        static string GetStringSafe(IDataRecord r, string name)
        {
            int idx = r.GetOrdinal(name);
            return r.IsDBNull(idx) ? string.Empty : r.GetString(idx);
        }

        static int GetInt32Safe(IDataRecord r, string name)
        {
            int idx = r.GetOrdinal(name);
            return r.IsDBNull(idx) ? 0 : r.GetInt32(idx);
        }

        static decimal GetDecimalSafe(IDataRecord r, string name)
        {
            int idx = r.GetOrdinal(name);
            return r.IsDBNull(idx) ? 0m : r.GetDecimal(idx);
        }

        static DateTime GetDateTimeSafe(IDataRecord r, string name)
        {
            int idx = r.GetOrdinal(name);
            return r.IsDBNull(idx) ? default : r.GetDateTime(idx);
        }

        static DateTime? GetNullableDateTime(IDataRecord r, string name)
        {
            int idx = r.GetOrdinal(name);
            return r.IsDBNull(idx) ? null : r.GetDateTime(idx);
        }

        static bool GetBooleanSafe(IDataRecord r, string name)
        {
            if (!HasColumn(r, name)) return false;
            int idx = r.GetOrdinal(name);
            return !r.IsDBNull(idx) && r.GetBoolean(idx);
        }

        var bus = new Bus
        {
            CountryNumber = GetStringSafe(reader, "CountryNumber"),
            BoardingNumber = GetStringSafe(reader, "BoardingNumber"),
            Brand = GetStringSafe(reader, "Brand"),
            PassengerCapacity = GetInt32Safe(reader, "PassengerCapacity"),
            YearOfManufacture = GetInt32Safe(reader, "YearOfManufacture"),
            Mileage = GetDecimalSafe(reader, "Mileage"),
            DateOfReceipt = GetDateTimeSafe(reader, "DateOfReceipt"),
            WriteoffDate = GetNullableDateTime(reader, "WriteoffDate"),
            CurrentStatusId = GetInt32Safe(reader, "CurrentStatusId"),
            IsDeleted = GetBooleanSafe(reader, "IsDeleted"),
            CreatedAt = GetDateTimeSafe(reader, "CreatedAt"),
            UpdatedAt = GetDateTimeSafe(reader, "UpdatedAt")
        };

        // Якщо є дані про статус (з JOIN)
        if (HasColumn(reader, "StatusId") && !reader.IsDBNull(reader.GetOrdinal("StatusId")))
        {
            bus.CurrentStatus = new BusStatus
            {
                StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                StatusName = HasColumn(reader, "StatusName") && !reader.IsDBNull(reader.GetOrdinal("StatusName"))
                    ? reader.GetString(reader.GetOrdinal("StatusName"))
                    : string.Empty,
                StatusDescription = HasColumn(reader, "StatusDescription") && !reader.IsDBNull(reader.GetOrdinal("StatusDescription"))
                    ? reader.GetString(reader.GetOrdinal("StatusDescription"))
                    : null
            };
        }

        return bus;
    }
}