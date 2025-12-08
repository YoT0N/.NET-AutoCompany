using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Seeders
{
    public class ExaminationSeeder : IDataSeeder
    {
        private readonly IMongoCollection<PhysicalExamination> _examinations;
        private readonly IMongoCollection<Personnel> _personnel;

        public ExaminationSeeder(MongoDbContext context)
        {
            _examinations = context.Examinations;
            _personnel = context.Personnel;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var existingCount = await _examinations.CountDocumentsAsync(
                FilterDefinition<PhysicalExamination>.Empty,
                cancellationToken: cancellationToken);

            if (existingCount > 0) return;

            var personnelList = await _personnel.Find(FilterDefinition<Personnel>.Empty)
                .Limit(3)
                .ToListAsync(cancellationToken);

            if (!personnelList.Any()) return;

            var seedData = new List<PhysicalExamination>
            {
                new PhysicalExamination(
                    personnelId: personnelList[0].PersonnelId,
                    examDate: new DateTime(2024, 6, 15),
                    result: "Passed",
                    doctorName: "Dr. Олена Іваненко",
                    metrics: new ExaminationMetricsVO(
                        height: 178,
                        weight: 82,
                        bloodPressure: "120/80",
                        vision: "1.0/1.0"
                    ),
                    notes: "Здоровий, придатний до роботи"
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                },

                new PhysicalExamination(
                    personnelId: personnelList[1].PersonnelId,
                    examDate: new DateTime(2024, 7, 20),
                    result: "Passed",
                    doctorName: "Dr. Петро Сидоренко",
                    metrics: new ExaminationMetricsVO(
                        height: 165,
                        weight: 62,
                        bloodPressure: "115/75",
                        vision: "1.0/0.9"
                    ),
                    notes: "Без зауважень"
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                },

                new PhysicalExamination(
                    personnelId: personnelList[2].PersonnelId,
                    examDate: new DateTime(2024, 5, 10),
                    result: "Passed",
                    doctorName: "Dr. Олена Іваненко",
                    metrics: new ExaminationMetricsVO(
                        height: 182,
                        weight: 88,
                        bloodPressure: "125/82",
                        vision: "0.9/1.0"
                    ),
                    notes: "Рекомендовано спостереження за тиском"
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                }
            };

            await _examinations.InsertManyAsync(seedData, cancellationToken: cancellationToken);
        }
    }
}