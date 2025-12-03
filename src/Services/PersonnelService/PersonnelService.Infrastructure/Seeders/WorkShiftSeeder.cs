using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Seeders
{
    public class WorkShiftSeeder : IDataSeeder
    {
        private readonly IMongoCollection<WorkShiftLog> _workShifts;
        private readonly IMongoCollection<Personnel> _personnel;

        public WorkShiftSeeder(MongoDbContext context)
        {
            _workShifts = context.WorkShifts;
            _personnel = context.Personnel;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var existingCount = await _workShifts.CountDocumentsAsync(
                FilterDefinition<WorkShiftLog>.Empty,
                cancellationToken: cancellationToken);

            if (existingCount > 0) return;

            var personnelList = await _personnel.Find(FilterDefinition<Personnel>.Empty)
                .Limit(3)
                .ToListAsync(cancellationToken);

            if (!personnelList.Any()) return;

            var seedData = new List<WorkShiftLog>
            {
                new WorkShiftLog(
                    personnelId: personnelList[0].PersonnelId,
                    shiftDate: DateTime.UtcNow.Date.AddDays(-2),
                    startTime: "06:00",
                    endTime: "14:00",
                    bus: new BusInfoVO("AA1234BB", "Mercedes-Benz"),
                    route: new RouteInfoVO("15А", 28.5)
                )
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Status = "Completed"
                },

                new WorkShiftLog(
                    personnelId: personnelList[0].PersonnelId,
                    shiftDate: DateTime.UtcNow.Date.AddDays(-1),
                    startTime: "06:00",
                    endTime: "14:00",
                    bus: new BusInfoVO("AA1234BB", "Mercedes-Benz"),
                    route: new RouteInfoVO("15А", 28.5)
                )
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Status = "Completed"
                },

                new WorkShiftLog(
                    personnelId: personnelList[0].PersonnelId,
                    shiftDate: DateTime.UtcNow.Date,
                    startTime: "06:00",
                    endTime: "14:00",
                    bus: new BusInfoVO("AA1234BB", "Mercedes-Benz"),
                    route: new RouteInfoVO("15А", 28.5)
                )
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Status = "InProgress"
                },

                new WorkShiftLog(
                    personnelId: personnelList[2].PersonnelId,
                    shiftDate: DateTime.UtcNow.Date,
                    startTime: "14:00",
                    endTime: "22:00",
                    bus: new BusInfoVO("BB5678CC", "Volkswagen"),
                    route: new RouteInfoVO("23", 35.2)
                )
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Status = "Scheduled"
                }
            };

            await _workShifts.InsertManyAsync(seedData, cancellationToken: cancellationToken);
        }
    }
}