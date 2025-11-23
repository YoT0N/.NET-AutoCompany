using Dapper;
using System.Data;
using TechnicalService.Core.Entities;
using TechnicalService.Core.Interfaces;
using TechnicalService.Infrastructure.Data;

namespace TechnicalService.Infrastructure.Repositories;

public class ExaminationRepository : IExaminationRepository
{
    private readonly DapperContext _context;

    public ExaminationRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TechnicalExamination>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM TechnicalExamination 
            ORDER BY ExaminationDate DESC";

        return await connection.QueryAsync<TechnicalExamination>(sql);
    }

    public async Task<TechnicalExamination?> GetByIdAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = "SELECT * FROM TechnicalExamination WHERE ExaminationId = @ExaminationId";

        return await connection.QueryFirstOrDefaultAsync<TechnicalExamination>(
            sql,
            new { ExaminationId = id }
        );
    }

    public async Task<int> AddAsync(TechnicalExamination entity)
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

        return await connection.ExecuteScalarAsync<int>(sql, entity);
    }

    public async Task<int> UpdateAsync(TechnicalExamination entity)
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

        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(object id)
    {
        using var connection = _context.CreateConnection();

        var sql = "DELETE FROM TechnicalExamination WHERE ExaminationId = @ExaminationId";

        return await connection.ExecuteAsync(sql, new { ExaminationId = id });
    }

    public async Task<IEnumerable<TechnicalExamination>> GetExaminationsByBusAsync(string countryNumber)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM TechnicalExamination 
            WHERE BusCountryNumber = @CountryNumber 
            ORDER BY ExaminationDate DESC";

        return await connection.QueryAsync<TechnicalExamination>(
            sql,
            new { CountryNumber = countryNumber }
        );
    }

    public async Task<IEnumerable<TechnicalExamination>> GetFailedExaminationsAsync()
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT * FROM TechnicalExamination 
            WHERE ExaminationResult = 'Failed' 
            ORDER BY ExaminationDate DESC";

        return await connection.QueryAsync<TechnicalExamination>(sql);
    }

    public async Task<TechnicalExamination?> GetExaminationWithPartsAsync(long examinationId)
    {
        using var connection = _context.CreateConnection();

        var sql = @"
            SELECT e.*, erp.*, rp.*
            FROM TechnicalExamination e
            LEFT JOIN ExaminationRepairPart erp ON e.ExaminationId = erp.ExaminationId
            LEFT JOIN RepairPart rp ON erp.PartId = rp.PartId
            WHERE e.ExaminationId = @ExaminationId";

        var examinationDictionary = new Dictionary<long, TechnicalExamination>();

        var result = await connection.QueryAsync<TechnicalExamination, ExaminationRepairPart, RepairPart, TechnicalExamination>(
            sql,
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
            new { ExaminationId = examinationId },
            splitOn: "ExaminationId,PartId"
        );

        return examinationDictionary.Values.FirstOrDefault();
    }

    public async Task<long> CreateExaminationWithPartsAsync(
        TechnicalExamination examination,
        List<ExaminationRepairPart> parts)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_BusCountryNumber", examination.BusCountryNumber);
            parameters.Add("p_ExaminationDate", examination.ExaminationDate);
            parameters.Add("p_ExaminationResult", examination.ExaminationResult);
            parameters.Add("p_SentForRepair", examination.SentForRepair);
            parameters.Add("p_RepairPrice", examination.RepairPrice);
            parameters.Add("p_MechanicName", examination.MechanicName);
            parameters.Add("p_Notes", examination.Notes);
            parameters.Add("p_ExaminationId", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_CreateExamination",
                parameters,
                transaction,
                commandType: CommandType.StoredProcedure
            );

            var examinationId = parameters.Get<long>("p_ExaminationId");

            if (parts?.Any() == true)
            {
                foreach (var part in parts)
                {
                    var partSql = @"
                        INSERT INTO ExaminationRepairPart (ExaminationId, PartId, Quantity)
                        VALUES (@ExaminationId, @PartId, @Quantity)";

                    await connection.ExecuteAsync(
                        partSql,
                        new
                        {
                            ExaminationId = examinationId,
                            part.PartId,
                            part.Quantity
                        },
                        transaction
                    );
                }
            }

            transaction.Commit();
            return examinationId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}