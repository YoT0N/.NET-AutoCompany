using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Domain.ValueObjects;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Seeders
{
    public class PersonnelSeeder : IDataSeeder
    {
        private readonly IMongoCollection<Personnel> _personnel;

        public PersonnelSeeder(MongoDbContext context)
        {
            _personnel = context.Personnel;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var existingCount = await _personnel.CountDocumentsAsync(
                FilterDefinition<Personnel>.Empty,
                cancellationToken: cancellationToken);

            if (existingCount > 0) return;

            var seedData = new List<Personnel>
            {
                new Personnel(
                    personnelId: 1,
                    fullName: "Іван Петренко",
                    birthDate: new DateTime(1985, 5, 15),
                    position: "Driver",
                    contacts: new PersonnelContactsVO(
                        "+380501234567",
                        "ivan.petrenko@example.com",
                        "Київ, вул. Хрещатик, 1"
                    )
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                },

                new Personnel(
                    personnelId: 2,
                    fullName: "Марія Коваленко",
                    birthDate: new DateTime(1990, 8, 22),
                    position: "Conductor",
                    contacts: new PersonnelContactsVO(
                        "+380502345678",
                        "maria.kovalenko@example.com",
                        "Київ, вул. Саксаганського, 5"
                    )
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                },

                new Personnel(
                    personnelId: 3,
                    fullName: "Олександр Шевченко",
                    birthDate: new DateTime(1982, 3, 10),
                    position: "Driver",
                    contacts: new PersonnelContactsVO(
                        "+380503456789",
                        "oleksandr.shevchenko@example.com",
                        "Київ, просп. Перемоги, 10"
                    )
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                }
            };

            await _personnel.InsertManyAsync(seedData, cancellationToken: cancellationToken);
        }
    }
}