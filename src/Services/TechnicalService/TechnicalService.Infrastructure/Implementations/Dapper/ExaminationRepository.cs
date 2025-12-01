using Dapper;
using System.Data;
using TechnicalService.Domain.Entities;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations.Dapper;

public class ExaminationRepository : IExaminationRepository
{
    private readonly DapperContext _context;

    public ExaminationRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TechnicalExamination>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM TechnicalExamination 
            ORDER BY ExaminationDate DESC";

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<TechnicalExamination>(command);
    }

    public async Task<TechnicalExamination?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = "SELECT * FROM TechnicalExamination WHERE ExaminationId = @ExaminationId";

        var command = new CommandDefinition(
            sql,
            new { ExaminationId = id },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<TechnicalExamination>(command);
    }

    public async Task<int> AddAsync(
        TechnicalExamination entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            INSERT INTO TechnicalExamination 
            (BusCountryNumber, ExaminationDate, ExaminationResult, SentForRepair, 
             RepairPrice, MechanicName, Notes)
            VALUES 
            (@BusCountryNumber, @ExaminationDate, @ExaminationResult, @SentForRepair, 
             @RepairPrice, @MechanicName, @Notes);
            SELECT LAST_INSERT_ID();";

        var command = new CommandDefinition(
            sql,
            entity,
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<int> UpdateAsync(
        TechnicalExamination entity,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            UPDATE TechnicalExamination 
            SET ExaminationDate = @ExaminationDate,
                ExaminationResult = @ExaminationResult,
                SentForRepair = @SentForRepair,
                RepairPrice = @RepairPrice,
                MechanicName = @MechanicName,
                Notes = @Notes
            WHERE ExaminationId = @ExaminationId";

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

        var sql = "DELETE FROM TechnicalExamination WHERE ExaminationId = @ExaminationId";

        var command = new CommandDefinition(
            sql,
            new { ExaminationId = id },
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<TechnicalExamination>> GetExaminationsByBusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM TechnicalExamination 
            WHERE BusCountryNumber = @CountryNumber 
            ORDER BY ExaminationDate DESC";

        var command = new CommandDefinition(
            sql,
            new { CountryNumber = countryNumber },
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<TechnicalExamination>(command);
    }

    public async Task<IEnumerable<TechnicalExamination>> GetFailedExaminationsAsync(
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM TechnicalExamination 
            WHERE ExaminationResult = 'Failed' 
            ORDER BY ExaminationDate DESC";

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<TechnicalExamination>(command);
    }

    public async Task<TechnicalExamination?> GetExaminationWithPartsAsync(
        long examinationId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT e.*, erp.*, rp.*
            FROM TechnicalExamination e
            LEFT JOIN ExaminationRepairPart erp ON e.ExaminationId = erp.ExaminationId
            LEFT JOIN RepairPart rp ON erp.PartId = rp.PartId
            WHERE e.ExaminationId = @ExaminationId";

        var examinationDictionary = new Dictionary<long, TechnicalExamination>();

        var command = new CommandDefinition(
            sql,
            new { ExaminationId = examinationId },
            cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<TechnicalExamination, ExaminationRepairPart, RepairPart, TechnicalExamination>(
            command,
            (examination, examinationPart, part) =>
            {
                if (!examinationDictionary.TryGetValue(examination.ExaminationId, out var examinationEntry))
                {
                    examinationEntry = examination;
                    examinationEntry.RepairParts = new List<ExaminationRepairPart>();
                    examinationDictionary.Add(examination.ExaminationId, examinationEntry);
                }

                if (examinationPart != null && part != null)
                {
                    examinationPart.Part = part;
                    examinationEntry.RepairParts.Add(examinationPart);
                }

                return examinationEntry;
            },
            splitOn: "ExaminationId,PartId"
        );

        return examinationDictionary.Values.FirstOrDefault();
    }

    // ========== ВАРІАНТ 2A: Метод з інтерфейсу (створює власну транзакцію) ==========

    /// <summary>
    /// Створення огляду з запчастинами - метод створює ВЛАСНУ транзакцію
    /// Використовується для простих викликів БЕЗ координації з іншими операціями
    /// </summary>
    public async Task<long> CreateExaminationWithPartsAsync(
        TechnicalExamination examination,
        List<ExaminationRepairPart> parts,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        try
        {
            var examinationId = await CreateExaminationWithPartsInternalAsync(
                examination,
                parts,
                connection,
                transaction,
                cancellationToken);

            transaction.Commit();
            return examinationId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // ========== ВАРІАНТ 2B: Метод для UoW (приймає зовнішню транзакцію) ==========

    /// <summary>
    /// Створення огляду з запчастинами - приймає ЗОВНІШНЮ транзакцію з UoW
    /// Використовується для координації з іншими операціями в рамках однієї транзакції
    /// </summary>
    public async Task<long> CreateExaminationWithPartsAsync(
        TechnicalExamination examination,
        List<ExaminationRepairPart> parts,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return await CreateExaminationWithPartsInternalAsync(
            examination,
            parts,
            connection,
            transaction,
            cancellationToken);
    }

    // ========== ВНУТРІШНІЙ МЕТОД (спільна логіка) ==========

    /// <summary>
    /// Внутрішня реалізація створення огляду з запчастинами
    /// </summary>
    private async Task<long> CreateExaminationWithPartsInternalAsync(
        TechnicalExamination examination,
        List<ExaminationRepairPart> parts,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        // 1. Виклик збереженої процедури для створення огляду
        var parameters = new DynamicParameters();
        parameters.Add("p_BusCountryNumber", examination.BusCountryNumber);
        parameters.Add("p_ExaminationDate", examination.ExaminationDate);
        parameters.Add("p_ExaminationResult", examination.ExaminationResult);
        parameters.Add("p_SentForRepair", examination.SentForRepair);
        parameters.Add("p_RepairPrice", examination.RepairPrice);
        parameters.Add("p_MechanicName", examination.MechanicName);
        parameters.Add("p_Notes", examination.Notes);
        parameters.Add("p_ExaminationId", dbType: DbType.Int64, direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            "sp_CreateExamination",
            parameters,
            transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        var examinationId = parameters.Get<long>("p_ExaminationId");

        // 2. Додавання запчастин
        if (parts?.Any() == true)
        {
            foreach (var part in parts)
            {
                var partSql = @"
                    INSERT INTO ExaminationRepairPart (ExaminationId, PartId, Quantity)
                    VALUES (@ExaminationId, @PartId, @Quantity)";

                var partCommand = new CommandDefinition(
                    partSql,
                    new
                    {
                        ExaminationId = examinationId,
                        part.PartId,
                        part.Quantity
                    },
                    transaction,
                    cancellationToken: cancellationToken);

                await connection.ExecuteAsync(partCommand);
            }
        }

        return examinationId;
    }
}