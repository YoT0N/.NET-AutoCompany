using TechnicalService.Core.DTOs;

namespace TechnicalService.Application.Interfaces;

public interface IExaminationService
{
    Task<IEnumerable<ExaminationDto>> GetAllExaminationsAsync();
    Task<ExaminationDto?> GetExaminationByIdAsync(long examinationId);
    Task<IEnumerable<ExaminationDto>> GetExaminationsByBusAsync(string countryNumber);
    Task<IEnumerable<ExaminationDto>> GetFailedExaminationsAsync();
    Task<ExaminationDto?> GetExaminationWithPartsAsync(long examinationId);
    Task<long> CreateExaminationAsync(CreateExaminationDto createExaminationDto);
    Task<int> UpdateExaminationAsync(long examinationId, CreateExaminationDto updateExaminationDto);
    Task<int> DeleteExaminationAsync(long examinationId);
}