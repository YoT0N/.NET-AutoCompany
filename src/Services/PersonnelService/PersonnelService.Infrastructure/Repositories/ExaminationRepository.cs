using MongoDB.Driver;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;
using PersonnelService.Infrastructure.Data;

namespace PersonnelService.Infrastructure.Repositories
{
    public class ExaminationRepository : IExaminationRepository
    {
        private readonly IMongoCollection<PhysicalExamination> _collection;

        public ExaminationRepository(MongoDbContext context)
        {
            _collection = context.PhysicalExaminations;
        }

        public async Task<IEnumerable<PhysicalExamination>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<PhysicalExamination?> GetByIdAsync(string id)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PhysicalExamination>> GetByPersonnelIdAsync(int personnelId)
        {
            return await _collection
                .Find(e => e.PersonnelId == personnelId)
                .SortByDescending(e => e.ExamDate)
                .ToListAsync();
        }

        public async Task<PhysicalExamination?> GetLatestByPersonnelIdAsync(int personnelId)
        {
            return await _collection
                .Find(e => e.PersonnelId == personnelId)
                .SortByDescending(e => e.ExamDate)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PhysicalExamination>> GetByResultAsync(string result)
        {
            return await _collection.Find(e => e.Result == result).ToListAsync();
        }

        public async Task<IEnumerable<PhysicalExamination>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _collection
                .Find(e => e.ExamDate >= startDate && e.ExamDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PhysicalExamination>> GetByDoctorAsync(string doctorName)
        {
            return await _collection.Find(e => e.DoctorName == doctorName).ToListAsync();
        }

        public async Task<PhysicalExamination> CreateAsync(PhysicalExamination examination)
        {
            await _collection.InsertOneAsync(examination);
            return examination;
        }

        public async Task<bool> UpdateAsync(string id, PhysicalExamination examination)
        {
            var result = await _collection.ReplaceOneAsync(e => e.Id == id, examination);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(e => e.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByPersonnelIdAsync(int personnelId)
        {
            var result = await _collection.DeleteManyAsync(e => e.PersonnelId == personnelId);
            return result.DeletedCount > 0;
        }
    }
}