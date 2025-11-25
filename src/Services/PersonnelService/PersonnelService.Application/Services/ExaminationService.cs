using PersonnelService.Application.Interfaces;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;

namespace PersonnelService.Application.Services
{
    public class ExaminationService : IExaminationService
    {
        private readonly IExaminationRepository _examinationRepository;

        public ExaminationService(IExaminationRepository examinationRepository)
        {
            _examinationRepository = examinationRepository;
        }

        public async Task<IEnumerable<PhysicalExamination>> GetAllExaminationsAsync()
        {
            return await _examinationRepository.GetAllAsync();
        }

        public async Task<PhysicalExamination?> GetExaminationByIdAsync(string id)
        {
            return await _examinationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PhysicalExamination>> GetExaminationsByPersonnelIdAsync(int personnelId)
        {
            return await _examinationRepository.GetByPersonnelIdAsync(personnelId);
        }

        public async Task<PhysicalExamination?> GetLatestExaminationByPersonnelIdAsync(int personnelId)
        {
            return await _examinationRepository.GetLatestByPersonnelIdAsync(personnelId);
        }

        public async Task<IEnumerable<PhysicalExamination>> GetExaminationsByResultAsync(string result)
        {
            return await _examinationRepository.GetByResultAsync(result);
        }

        public async Task<IEnumerable<PhysicalExamination>> GetExaminationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _examinationRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<PhysicalExamination>> GetExaminationsByDoctorAsync(string doctorName)
        {
            return await _examinationRepository.GetByDoctorAsync(doctorName);
        }

        public async Task<PhysicalExamination> CreateExaminationAsync(PhysicalExamination examination)
        {
            return await _examinationRepository.CreateAsync(examination);
        }

        public async Task<bool> UpdateExaminationAsync(string id, PhysicalExamination examination)
        {
            return await _examinationRepository.UpdateAsync(id, examination);
        }

        public async Task<bool> DeleteExaminationAsync(string id)
        {
            return await _examinationRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteExaminationsByPersonnelIdAsync(int personnelId)
        {
            return await _examinationRepository.DeleteByPersonnelIdAsync(personnelId);
        }
    }
}