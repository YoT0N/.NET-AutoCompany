using MongoDB.Bson;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.Interfaces;
using PersonnelService.Infrastructure.Context;

namespace PersonnelService.Infrastructure.Seeders
{
    public class DocumentSeeder : IDataSeeder
    {
        private readonly IMongoCollection<PersonnelDocument> _documents;
        private readonly IMongoCollection<Personnel> _personnel;

        public DocumentSeeder(MongoDbContext context)
        {
            _documents = context.Documents;
            _personnel = context.Personnel;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var existingCount = await _documents.CountDocumentsAsync(
                FilterDefinition<PersonnelDocument>.Empty,
                cancellationToken: cancellationToken);

            if (existingCount > 0) return;

            var personnelList = await _personnel.Find(FilterDefinition<Personnel>.Empty)
                .Limit(3)
                .ToListAsync(cancellationToken);

            if (!personnelList.Any()) return;

            var seedData = new List<PersonnelDocument>
            {
                new PersonnelDocument(
                    personnelId: personnelList[0].PersonnelId,
                    docType: "DriverLicense",
                    fileName: "driver_license_001.pdf",
                    fileSize: 245000,
                    mimeType: "application/pdf",
                    issuedOn: new DateTime(2020, 1, 15),
                    validUntil: new DateTime(2030, 1, 15)
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                },

                new PersonnelDocument(
                    personnelId: personnelList[0].PersonnelId,
                    docType: "MedicalCertificate",
                    fileName: "medical_cert_001.pdf",
                    fileSize: 180000,
                    mimeType: "application/pdf",
                    issuedOn: new DateTime(2024,6, 1),
                    validUntil: new DateTime(2025, 6, 1))
                    {
                    Id = ObjectId.GenerateNewId().ToString()
                    },

                new PersonnelDocument(
                    personnelId: personnelList[1].PersonnelId,
                    docType: "Passport",
                    fileName: "passport_002.pdf",
                    fileSize: 320000,
                    mimeType: "application/pdf",
                    issuedOn: new DateTime(2019, 3, 10),
                    validUntil: new DateTime(2029, 3, 10)
                )
                {
                    Id = ObjectId.GenerateNewId().ToString()
                }
            };

            await _documents.InsertManyAsync(seedData, cancellationToken: cancellationToken);
        }
    }
}