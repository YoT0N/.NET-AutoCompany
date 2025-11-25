using MongoDB.Driver;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;
using PersonnelService.Infrastructure.Data;

namespace PersonnelService.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IMongoCollection<PersonnelDocument> _collection;

        public DocumentRepository(MongoDbContext context)
        {
            _collection = context.PersonnelDocuments;
        }

        public async Task<IEnumerable<PersonnelDocument>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<PersonnelDocument?> GetByIdAsync(string id)
        {
            return await _collection.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PersonnelDocument>> GetByPersonnelIdAsync(int personnelId)
        {
            return await _collection.Find(d => d.PersonnelId == personnelId).ToListAsync();
        }

        public async Task<IEnumerable<PersonnelDocument>> GetByDocTypeAsync(string docType)
        {
            return await _collection.Find(d => d.DocType == docType).ToListAsync();
        }

        public async Task<IEnumerable<PersonnelDocument>> GetExpiredDocumentsAsync(DateTime beforeDate)
        {
            return await _collection.Find(d => d.ValidUntil < beforeDate).ToListAsync();
        }

        public async Task<IEnumerable<PersonnelDocument>> GetExpiringDocumentsAsync(DateTime withinDate)
        {
            var today = DateTime.UtcNow.Date;
            return await _collection.Find(d => d.ValidUntil >= today && d.ValidUntil <= withinDate).ToListAsync();
        }

        public async Task<PersonnelDocument> CreateAsync(PersonnelDocument document)
        {
            await _collection.InsertOneAsync(document);
            return document;
        }

        public async Task<bool> UpdateAsync(string id, PersonnelDocument document)
        {
            var result = await _collection.ReplaceOneAsync(d => d.Id == id, document);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByPersonnelIdAsync(int personnelId)
        {
            var result = await _collection.DeleteManyAsync(d => d.PersonnelId == personnelId);
            return result.DeletedCount > 0;
        }
    }
}