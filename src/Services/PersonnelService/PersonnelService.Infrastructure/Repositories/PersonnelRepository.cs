using MongoDB.Driver;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;
using PersonnelService.Infrastructure.Data;

namespace PersonnelService.Infrastructure.Repositories
{
    public class PersonnelRepository : IPersonnelRepository
    {
        private readonly IMongoCollection<Personnel> _collection;

        public PersonnelRepository(MongoDbContext context)
        {
            _collection = context.Personnel;
        }

        public async Task<IEnumerable<Personnel>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Personnel?> GetByIdAsync(string id)
        {
            return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Personnel?> GetByPersonnelIdAsync(int personnelId)
        {
            return await _collection.Find(p => p.PersonnelId == personnelId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Personnel>> GetByPositionAsync(string position)
        {
            return await _collection.Find(p => p.Position == position).ToListAsync();
        }

        public async Task<IEnumerable<Personnel>> GetByStatusAsync(string status)
        {
            return await _collection.Find(p => p.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Personnel>> GetActivePersonnelAsync()
        {
            return await _collection.Find(p => p.Status == "Active").ToListAsync();
        }

        public async Task<Personnel> CreateAsync(Personnel personnel)
        {
            await _collection.InsertOneAsync(personnel);
            return personnel;
        }

        public async Task<bool> UpdateAsync(string id, Personnel personnel)
        {
            personnel.UpdatedAt = DateTime.UtcNow;
            var result = await _collection.ReplaceOneAsync(p => p.Id == id, personnel);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UpdateStatusAsync(string id, string status)
        {
            var update = Builders<Personnel>.Update
                .Set(p => p.Status, status)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(p => p.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateContactsAsync(string id, PersonnelContacts contacts)
        {
            var update = Builders<Personnel>.Update
                .Set(p => p.Contacts, contacts)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(p => p.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddDocumentAsync(string id, PersonnelDocumentInfo document)
        {
            var update = Builders<Personnel>.Update
                .Push(p => p.Documents, document)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(p => p.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<int> GetNextPersonnelIdAsync()
        {
            var maxPersonnel = await _collection
                .Find(_ => true)
                .SortByDescending(p => p.PersonnelId)
                .Limit(1)
                .FirstOrDefaultAsync();

            return maxPersonnel?.PersonnelId + 1 ?? 1;
        }
    }
}