using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Repositories
{
    public class DocumentRepository : MongoRepository<PersonnelDocument>, IDocumentRepository
    {
        public DocumentRepository(MongoDbContext context)
            : base(context.Documents) { }

        public async Task<IReadOnlyCollection<PersonnelDocument>> GetByPersonnelIdAsync(int personnelId)
        {
            var list = await _collection
                .Find(d => d.PersonnelId == personnelId)
                .SortByDescending(d => d.UploadedAt)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<PersonnelDocument>> GetByDocTypeAsync(string docType)
        {
            var list = await _collection
                .Find(d => d.DocType == docType)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<PersonnelDocument>> GetExpiredDocumentsAsync(DateTime? beforeDate = null)
        {
            var date = beforeDate ?? DateTime.UtcNow;
            var list = await _collection
                .Find(d => d.ValidUntil.HasValue && d.ValidUntil.Value < date)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<PersonnelDocument>> GetExpiringDocumentsAsync(int daysThreshold = 30)
        {
            var today = DateTime.UtcNow;
            var futureDate = today.AddDays(daysThreshold);

            var list = await _collection
                .Find(d => d.ValidUntil.HasValue
                    && d.ValidUntil.Value > today
                    && d.ValidUntil.Value <= futureDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<PersonnelDocument>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var list = await _collection
                .Find(d => d.UploadedAt >= startDate && d.UploadedAt <= endDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task DeleteByPersonnelIdAsync(int personnelId)
        {
            await _collection.DeleteManyAsync(d => d.PersonnelId == personnelId);
        }

        public async Task<bool> HasDocumentTypeAsync(int personnelId, string docType, CancellationToken cancellationToken = default)
        {
            var count = await _collection
                .CountDocumentsAsync(
                    d => d.PersonnelId == personnelId && d.DocType == docType,
                    cancellationToken: cancellationToken);
            return count > 0;
        }

        public async Task<Dictionary<string, int>> GetDocumentCountByTypeAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$docType" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return results.ToDictionary(
                doc => doc["_id"].AsString,
                doc => doc["count"].AsInt32
            );
        }

        public async Task<IReadOnlyCollection<PersonnelDocument>> SearchAsync(
            int? personnelId = null,
            string? docType = null,
            bool? onlyExpired = null,
            bool? onlyExpiring = null,
            int skip = 0,
            int limit = 10)
        {
            var filter = Builders<PersonnelDocument>.Filter.Empty;

            if (personnelId.HasValue)
                filter &= Builders<PersonnelDocument>.Filter.Eq(d => d.PersonnelId, personnelId.Value);

            if (!string.IsNullOrWhiteSpace(docType))
                filter &= Builders<PersonnelDocument>.Filter.Eq(d => d.DocType, docType);

            if (onlyExpired == true)
            {
                filter &= Builders<PersonnelDocument>.Filter.And(
                    Builders<PersonnelDocument>.Filter.Ne(d => d.ValidUntil, null),
                    Builders<PersonnelDocument>.Filter.Lt(d => d.ValidUntil, DateTime.UtcNow)
                );
            }

            if (onlyExpiring == true)
            {
                var today = DateTime.UtcNow;
                var futureDate = today.AddDays(30);
                filter &= Builders<PersonnelDocument>.Filter.And(
                    Builders<PersonnelDocument>.Filter.Ne(d => d.ValidUntil, null),
                    Builders<PersonnelDocument>.Filter.Gt(d => d.ValidUntil, today),
                    Builders<PersonnelDocument>.Filter.Lte(d => d.ValidUntil, futureDate)
                );
            }

            var list = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();

            return list.AsReadOnly();
        }
    }
}