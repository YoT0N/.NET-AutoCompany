using PersonnelService.Core.Models;

namespace PersonnelService.Core.Interfaces
{
    public interface IExaminationRepository
    {
        Task<IEnumerable<PhysicalExamination>> GetAllAsync();
        Task<PhysicalExamination?> GetByIdAsync(string id);
        Task<IEnumerable<PhysicalExamination>> GetByPersonnelIdAsync(int personnelId);
        Task<PhysicalExamination?> GetLatestByPersonnelIdAsync(int personnelId);
        Task<IEnumerable<PhysicalExamination>> GetByResultAsync(string result);
        Task<IEnumerable<PhysicalExamination>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PhysicalExamination>> GetByDoctorAsync(string doctorName);
        Task<PhysicalExamination> CreateAsync(PhysicalExamination examination);
        Task<bool> UpdateAsync(string id, PhysicalExamination examination);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByPersonnelIdAsync(int personnelId);
    }
}