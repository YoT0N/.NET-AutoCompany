using TechnicalService.Core.Entities;

namespace TechnicalService.Core.Interfaces;

public interface IExaminationRepository : IRepository<TechnicalExamination>
{
    Task<IEnumerable<TechnicalExamination>> GetExaminationsByBusAsync(string countryNumber);
    Task<IEnumerable<TechnicalExamination>> GetFailedExaminationsAsync();
    Task<TechnicalExamination?> GetExaminationWithPartsAsync(long examinationId);
    Task<long> CreateExaminationWithPartsAsync(TechnicalExamination examination,
        List<ExaminationRepairPart> parts);
}