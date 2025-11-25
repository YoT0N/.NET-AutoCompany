using MongoDB.Driver;
using PersonnelService.Core.Models;

namespace PersonnelService.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<Personnel> Personnel =>
            _database.GetCollection<Personnel>("Personnel");

        public IMongoCollection<PersonnelDocument> PersonnelDocuments =>
            _database.GetCollection<PersonnelDocument>("PersonnelDocuments");

        public IMongoCollection<PhysicalExamination> PhysicalExaminations =>
            _database.GetCollection<PhysicalExamination>("PhysicalExamination");

        public IMongoCollection<WorkShiftLog> WorkShiftLogs =>
            _database.GetCollection<WorkShiftLog>("WorkShiftLog");
    }
}