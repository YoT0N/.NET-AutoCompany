using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Common;

namespace PersonnelService.Infrastructure.Repositories
{
    public class MongoRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public virtual async Task AddAsync(T entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = ObjectId.GenerateNewId().ToString();
            }

            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _collection.InsertOneAsync(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            await _collection.ReplaceOneAsync(
                e => e.Id == entity.Id,
                entity,
                new ReplaceOptions { IsUpsert = false }
            );
        }

        public virtual async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(e => e.Id == id);
        }

        public virtual async Task<T?> GetByIdAsync(string id)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            var list = await _collection.Find(_ => true).ToListAsync();
            return list.AsReadOnly();
        }

        public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(e => e.Id == id).AnyAsync(cancellationToken);
        }
    }
}