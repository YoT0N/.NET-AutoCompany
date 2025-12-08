using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Repositories
{
    public class PersonnelRepository : MongoRepository<Personnel>, IPersonnelRepository
    {
        public PersonnelRepository(MongoDbContext context)
            : base(context.Personnel) { }

        public async Task<Personnel?> GetByPersonnelIdAsync(int personnelId)
        {
            return await _collection
                .Find(p => p.PersonnelId == personnelId)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<Personnel>> GetByPositionAsync(string position)
        {
            var list = await _collection
                .Find(p => p.Position == position)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Personnel>> GetByStatusAsync(string status)
        {
            var list = await _collection
                .Find(p => p.Status == status)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<Personnel>> GetActivePersonnelAsync()
        {
            var list = await _collection
                .Find(p => p.Status == "Active")
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<bool> ExistsByPersonnelIdAsync(int personnelId, CancellationToken cancellationToken = default)
        {
            var count = await _collection
                .CountDocumentsAsync(p => p.PersonnelId == personnelId, cancellationToken: cancellationToken);
            return count > 0;
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

        public async Task<IReadOnlyCollection<Personnel>> SearchAsync(
            string? searchText = null,
            string? position = null,
            string? status = null,
            int skip = 0,
            int limit = 10)
        {
            var filter = Builders<Personnel>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(searchText))
                filter &= Builders<Personnel>.Filter.Text(searchText);

            if (!string.IsNullOrWhiteSpace(position))
                filter &= Builders<Personnel>.Filter.Eq(p => p.Position, position);

            if (!string.IsNullOrWhiteSpace(status))
                filter &= Builders<Personnel>.Filter.Eq(p => p.Status, status);

            var list = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();

            return list.AsReadOnly();
        }
    }
}