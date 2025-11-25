using PersonnelService.Core.Models;

namespace PersonnelService.Application.Interfaces
{
    public interface IExaminationService
    {
        Task<IEnumerable<PhysicalExamination>> GetAllExaminationsAsync();
        Task<PhysicalExamination?> GetExaminationByIdAsync(string id);
        Task<IEnumerable<PhysicalExamination>> GetExaminationsByPersonnelIdAsync(int personnelId);
        Task<PhysicalExamination?> GetLatestExaminationByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<PhysicalExamination>> GetExaminationsByResultAsync(string result);
        Task<IEnumerable<PhysicalExamination>> GetExaminationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PhysicalExamination>> GetExaminationsByDoctorAsync(string doctorName);
        Task<PhysicalExamination> CreateExaminationAsync(PhysicalExamination examination);
        Task<bool> UpdateExaminationAsync(string id, PhysicalExamination examination);
        Task<bool> DeleteExaminationAsync(string id);
        Task<bool> DeleteExaminationsByPersonnelIdAsync(int personnelId);
    }
}