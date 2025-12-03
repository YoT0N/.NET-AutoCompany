using PersonnelService.Domain.Entities;

namespace PersonnelService.Domain.Interfaces
{
    public interface IExaminationRepository
    {
        Task<PhysicalExamination?> GetByIdAsync(string id);
        Task<IReadOnlyCollection<PhysicalExamination>> GetAllAsync();
        Task<IReadOnlyCollection<PhysicalExamination>> GetByPersonnelIdAsync(int personnelId);
        Task<PhysicalExamination?> GetLatestByPersonnelIdAsync(int personnelId);
        Task<IReadOnlyCollection<PhysicalExamination>> GetByResultAsync(string result);
        Task<IReadOnlyCollection<PhysicalExamination>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IReadOnlyCollection<PhysicalExamination>> GetByDoctorAsync(string doctorName);

        Task AddAsync(PhysicalExamination examination);
        Task UpdateAsync(PhysicalExamination examination);
        Task DeleteAsync(string id);

        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> HasValidExaminationAsync(int personnelId, int validityDays = 365);
    }
}