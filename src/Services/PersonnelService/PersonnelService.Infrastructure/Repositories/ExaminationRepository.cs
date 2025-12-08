using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Repositories
{
    public class ExaminationRepository : MongoRepository<PhysicalExamination>, IExaminationRepository
    {
        public ExaminationRepository(MongoDbContext context)
            : base(context.Examinations) { }

        public async Task<IReadOnlyCollection<PhysicalExamination>> GetByPersonnelIdAsync(int personnelId)
        {
            var list = await _collection
                .Find(e => e.PersonnelId == personnelId)
                .SortByDescending(e => e.ExamDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<PhysicalExamination?> GetLatestByPersonnelIdAsync(int personnelId)
        {
            return await _collection
                .Find(e => e.PersonnelId == personnelId)
                .SortByDescending(e => e.ExamDate)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<PhysicalExamination>> GetByResultAsync(string result)
        {
            var list = await _collection
                .Find(e => e.Result == result)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<PhysicalExamination>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var list = await _collection
                .Find(e => e.ExamDate >= startDate && e.ExamDate <= endDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<PhysicalExamination>> GetByDoctorAsync(string doctorName)
        {
            var list = await _collection
                .Find(e => e.DoctorName == doctorName)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<bool> HasValidExaminationAsync(int personnelId, int validityDays = 365)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-validityDays);

            var count = await _collection
                .CountDocumentsAsync(e =>
                    e.PersonnelId == personnelId
                    && e.ExamDate >= cutoffDate
                    && e.Result == "Passed");

            return count > 0;
        }
    }
}