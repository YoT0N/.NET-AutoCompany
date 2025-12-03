using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PersonnelService.Domain.Entities;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Infrastructure.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb")
                ?? throw new InvalidOperationException("Connection string 'MongoDb' not found.");

            var databaseName = configuration["MongoDB:DatabaseName"] ?? "PersonnelServiceDb";

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);

            ConfigureMappings();
            MongoIndexes.Configure(_database);
        }

        public IMongoCollection<Personnel> Personnel =>
            _database.GetCollection<Personnel>("Personnel");

        public IMongoCollection<PersonnelDocument> Documents =>
            _database.GetCollection<PersonnelDocument>("Documents");

        public IMongoCollection<PhysicalExamination> Examinations =>
            _database.GetCollection<PhysicalExamination>("Examinations");

        public IMongoCollection<WorkShiftLog> WorkShifts =>
            _database.GetCollection<WorkShiftLog>("WorkShifts");

        public IClientSessionHandle StartSession() => _client.StartSession();

        private void ConfigureMappings()
        {
            // PersonnelContactsVO
            BsonClassMap.RegisterClassMap<PersonnelContactsVO>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(c => new PersonnelContactsVO(c.Phone, c.Email, c.Address));
            });

            // BusInfoVO
            BsonClassMap.RegisterClassMap<BusInfoVO>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(b => new BusInfoVO(b.BusCountryNumber, b.Brand));
            });

            // RouteInfoVO
            BsonClassMap.RegisterClassMap<RouteInfoVO>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(r => new RouteInfoVO(r.RouteNumber, r.DistanceKm));
            });

            // ExaminationMetricsVO
            BsonClassMap.RegisterClassMap<ExaminationMetricsVO>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(m => new ExaminationMetricsVO(
                    m.Height, m.Weight, m.BloodPressure, m.Vision));
            });
        }
    }
}