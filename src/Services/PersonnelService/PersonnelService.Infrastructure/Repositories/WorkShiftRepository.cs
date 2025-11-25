using MongoDB.Driver;
using PersonnelService.Core.Interfaces;
using PersonnelService.Core.Models;
using PersonnelService.Infrastructure.Data;

namespace PersonnelService.Infrastructure.Repositories
{
    public class WorkShiftRepository : IWorkShiftRepository
    {
        private readonly IMongoCollection<WorkShiftLog> _collection;

        public WorkShiftRepository(MongoDbContext context)
        {
            _collection = context.WorkShiftLogs;
        }

        public async Task<IEnumerable<WorkShiftLog>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<WorkShiftLog?> GetByIdAsync(string id)
        {
            return await _collection.Find(w => w.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WorkShiftLog>> GetByPersonnelIdAsync(int personnelId)
        {
            return await _collection
                .Find(w => w.PersonnelId == personnelId)
                .SortByDescending(w => w.ShiftDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkShiftLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _collection
                .Find(w => w.ShiftDate >= startDate && w.ShiftDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkShiftLog>> GetByPersonnelAndDateRangeAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            return await _collection
                .Find(w => w.PersonnelId == personnelId && w.ShiftDate >= startDate && w.ShiftDate <= endDate)
                .SortBy(w => w.ShiftDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkShiftLog>> GetByBusNumberAsync(string busCountryNumber)
        {
            return await _collection
                .Find(w => w.Bus.BusCountryNumber == busCountryNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkShiftLog>> GetByRouteNumberAsync(string routeNumber)
        {
            return await _collection
                .Find(w => w.Route.RouteNumber == routeNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkShiftLog>> GetByStatusAsync(string status)
        {
            return await _collection.Find(w => w.Status == status).ToListAsync();
        }

        public async Task<WorkShiftLog> CreateAsync(WorkShiftLog workShift)
        {
            await _collection.InsertOneAsync(workShift);
            return workShift;
        }

        public async Task<bool> UpdateAsync(string id, WorkShiftLog workShift)
        {
            var result = await _collection.ReplaceOneAsync(w => w.Id == id, workShift);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(w => w.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByPersonnelIdAsync(int personnelId)
        {
            var result = await _collection.DeleteManyAsync(w => w.PersonnelId == personnelId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UpdateStatusAsync(string id, string status)
        {
            var update = Builders<WorkShiftLog>.Update.Set(w => w.Status, status);
            var result = await _collection.UpdateOneAsync(w => w.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<double> GetTotalDistanceByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            var shifts = await GetByPersonnelAndDateRangeAsync(personnelId, startDate, endDate);
            return shifts.Sum(s => s.Route.DistanceKm);
        }

        public async Task<int> GetShiftCountByPersonnelAsync(int personnelId, DateTime startDate, DateTime endDate)
        {
            var count = await _collection.CountDocumentsAsync(
                w => w.PersonnelId == personnelId &&
                     w.ShiftDate >= startDate &&
                     w.ShiftDate <= endDate
            );
            return (int)count;
        }
    }
}