using TechnicalService.Bll.DTOs.Examination;

namespace TechnicalService.Bll.Interfaces;

public interface IExaminationService
{
    Task<IEnumerable<ExaminationDto>> GetAllExaminationsAsync(
        CancellationToken cancellationToken = default);

    Task<ExaminationDto> GetExaminationByIdAsync(
        long examinationId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ExaminationDto>> GetExaminationsByBusAsync(
        string countryNumber,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ExaminationDto>> GetFailedExaminationsAsync(
        CancellationToken cancellationToken = default);

    Task<ExaminationDto> GetExaminationWithPartsAsync(
        long examinationId,
        CancellationToken cancellationToken = default);

    Task<long> CreateExaminationAsync(
        CreateExaminationDto dto,
        CancellationToken cancellationToken = default);

    Task<int> UpdateExaminationAsync(
        long examinationId,
        UpdateExaminationDto dto,
        CancellationToken cancellationToken = default);

    Task<int> DeleteExaminationAsync(
        long examinationId,
        CancellationToken cancellationToken = default);
}