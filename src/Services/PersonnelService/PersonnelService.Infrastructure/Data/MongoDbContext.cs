using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PersonnelService.Core.Models;

namespace PersonnelService.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(IOptions<MongoDbSettings> options)
        {
            _settings = options.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoCollection<Personnel> Personnel =>
            _database.GetCollection<Personnel>(_settings.PersonnelCollectionName);

        public IMongoCollection<PersonnelDocument> PersonnelDocuments =>
            _database.GetCollection<PersonnelDocument>(_settings.DocumentCollectionName);

        public IMongoCollection<PhysicalExamination> PhysicalExaminations =>
            _database.GetCollection<PhysicalExamination>(_settings.ExaminationCollectionName);

        public IMongoCollection<WorkShiftLog> WorkShiftLogs =>
            _database.GetCollection<WorkShiftLog>(_settings.WorkShiftCollectionName);
    }
}
