using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Repositories
{
    public class WorkShiftRepository : MongoRepository<WorkShiftLog>, IWorkShiftRepository
    {
        public WorkShiftRepository(MongoDbContext context)
            : base(context.WorkShifts) { }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetByPersonnelIdAsync(int personnelId)
        {
            var list = await _collection
                .Find(w => w.PersonnelId == personnelId)
                .SortByDescending(w => w.ShiftDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var list = await _collection
                .Find(w => w.ShiftDate >= startDate && w.ShiftDate <= endDate)
                .SortBy(w => w.ShiftDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetByPersonnelAndDateRangeAsync(
            int personnelId,
            DateTime startDate,
            DateTime endDate)
        {
            var list = await _collection
                .Find(w => w.PersonnelId == personnelId
                    && w.ShiftDate >= startDate
                    && w.ShiftDate <= endDate)
                .SortBy(w => w.ShiftDate)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetByBusNumberAsync(string busCountryNumber)
        {
            var filter = Builders<WorkShiftLog>.Filter.Eq("bus.busCountryNumber", busCountryNumber);
            var list = await _collection.Find(filter).ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetByRouteNumberAsync(string routeNumber)
        {
            var filter = Builders<WorkShiftLog>.Filter.Eq("route.routeNumber", routeNumber);
            var list = await _collection.Find(filter).ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetByStatusAsync(string status)
        {
            var list = await _collection
                .Find(w => w.Status == status)
                .ToListAsync();
            return list.AsReadOnly();
        }

        public async Task<double> GetTotalDistanceByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "personnelId", personnelId },
                    { "shiftDate", new BsonDocument
                        {
                            { "$gte", startDate },
                            { "$lte", endDate }
                        }
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "totalDistance", new BsonDocument("$sum", "$route.distanceKm") }
                })
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return result?["totalDistance"].ToDouble() ?? 0;
        }

        public async Task<int> GetShiftCountByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            var count = await _collection
                .CountDocumentsAsync(w =>
                    w.PersonnelId == personnelId
                    && w.ShiftDate >= startDate
                    && w.ShiftDate <= endDate);

            return (int)count;
        }

        public async Task<IReadOnlyCollection<WorkShiftLog>> GetUpcomingShiftsAsync(int personnelId, int daysAhead = 7)
        {
            var today = DateTime.UtcNow.Date;
            var futureDate = today.AddDays(daysAhead);

            var list = await _collection
                .Find(w => w.PersonnelId == personnelId
                    && w.ShiftDate >= today
                    && w.ShiftDate <= futureDate
                    && w.Status == "Scheduled")
                .SortBy(w => w.ShiftDate)
                .ToListAsync();

            return list.AsReadOnly();
        }
    }
}