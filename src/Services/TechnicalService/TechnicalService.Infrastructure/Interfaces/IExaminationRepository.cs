using System.Data;
using TechnicalService.Domain.Entities;

namespace TechnicalService.Dal.Interfaces;

public interface IExaminationRepository : IRepository<TechnicalExamination>
{
    Task<IEnumerable<TechnicalExamination>> GetExaminationsByBusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TechnicalExamination>> GetFailedExaminationsAsync(
        CancellationToken cancellationToken = default);

    Task<TechnicalExamination?> GetExaminationWithPartsAsync(
        long examinationId,
        CancellationToken cancellationToken = default);

    Task<long> CreateExaminationWithPartsAsync(
        TechnicalExamination examination,
        List<ExaminationRepairPart> parts,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}