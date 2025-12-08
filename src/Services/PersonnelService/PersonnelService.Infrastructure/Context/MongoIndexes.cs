using MongoDB.Driver;
using PersonnelService.Domain.Entities;

namespace PersonnelService.Infrastructure.Context
{
    public static class MongoIndexes
    {
        public static void Configure(IMongoDatabase database)
        {
            var personnel = database.GetCollection<Personnel>("Personnel");
            var documents = database.GetCollection<PersonnelDocument>("Documents");
            var examinations = database.GetCollection<PhysicalExamination>("Examinations");
            var workShifts = database.GetCollection<WorkShiftLog>("WorkShifts");

            // Personnel indexes

            // Унікальний індекс по PersonnelId
            var personnelIdIndex = Builders<Personnel>.IndexKeys.Ascending(p => p.PersonnelId);
            personnel.Indexes.CreateOne(new CreateIndexModel<Personnel>(
                personnelIdIndex,
                new CreateIndexOptions { Unique = true }));

            // Text index для пошуку по FullName
            var personnelTextIndex = Builders<Personnel>.IndexKeys.Text(p => p.FullName);
            personnel.Indexes.CreateOne(new CreateIndexModel<Personnel>(personnelTextIndex));

            // Compound index для швидкого пошуку по Position та Status
            var personnelCompoundIndex = Builders<Personnel>.IndexKeys
                .Ascending(p => p.Position)
                .Ascending(p => p.Status);
            personnel.Indexes.CreateOne(new CreateIndexModel<Personnel>(personnelCompoundIndex));

            // Index по Status
            var statusIndex = Builders<Personnel>.IndexKeys.Ascending(p => p.Status);
            personnel.Indexes.CreateOne(new CreateIndexModel<Personnel>(statusIndex));

            // PersonnelDocument indexes

            // Compound index для PersonnelId та DocType
            var docCompoundIndex = Builders<PersonnelDocument>.IndexKeys
                .Ascending(d => d.PersonnelId)
                .Ascending(d => d.DocType);
            documents.Indexes.CreateOne(new CreateIndexModel<PersonnelDocument>(docCompoundIndex));

            // Index по ValidUntil для пошуку документів що закінчуються
            var validUntilIndex = Builders<PersonnelDocument>.IndexKeys.Ascending(d => d.ValidUntil);
            documents.Indexes.CreateOne(new CreateIndexModel<PersonnelDocument>(validUntilIndex));

            // Compound index для пошуку прострочених документів
            var expiredDocsIndex = Builders<PersonnelDocument>.IndexKeys
                .Ascending(d => d.PersonnelId)
                .Ascending(d => d.ValidUntil);
            documents.Indexes.CreateOne(new CreateIndexModel<PersonnelDocument>(expiredDocsIndex));

            // PhysicalExamination indexes

            // Compound index для PersonnelId та ExamDate
            var examCompoundIndex = Builders<PhysicalExamination>.IndexKeys
                .Ascending(e => e.PersonnelId)
                .Descending(e => e.ExamDate);
            examinations.Indexes.CreateOne(new CreateIndexModel<PhysicalExamination>(examCompoundIndex));

            // Index по Result
            var resultIndex = Builders<PhysicalExamination>.IndexKeys.Ascending(e => e.Result);
            examinations.Indexes.CreateOne(new CreateIndexModel<PhysicalExamination>(resultIndex));

            // Index по DoctorName
            var doctorIndex = Builders<PhysicalExamination>.IndexKeys.Ascending(e => e.DoctorName);
            examinations.Indexes.CreateOne(new CreateIndexModel<PhysicalExamination>(doctorIndex));

            // WorkShiftLog indexes

            // Compound index для PersonnelId та ShiftDate
            var shiftCompoundIndex = Builders<WorkShiftLog>.IndexKeys
                .Ascending(w => w.PersonnelId)
                .Descending(w => w.ShiftDate);
            workShifts.Indexes.CreateOne(new CreateIndexModel<WorkShiftLog>(shiftCompoundIndex));

            // Index по ShiftDate
            var shiftDateIndex = Builders<WorkShiftLog>.IndexKeys.Descending(w => w.ShiftDate);
            workShifts.Indexes.CreateOne(new CreateIndexModel<WorkShiftLog>(shiftDateIndex));

            // Index по Status
            var shiftStatusIndex = Builders<WorkShiftLog>.IndexKeys.Ascending(w => w.Status);
            workShifts.Indexes.CreateOne(new CreateIndexModel<WorkShiftLog>(shiftStatusIndex));

            // Compound index для пошуку по Bus.BusCountryNumber
            var busNumberIndex = Builders<WorkShiftLog>.IndexKeys
                .Ascending("bus.busCountryNumber");
            workShifts.Indexes.CreateOne(new CreateIndexModel<WorkShiftLog>(busNumberIndex));

            // Compound index для пошуку по Route.RouteNumber
            var routeNumberIndex = Builders<WorkShiftLog>.IndexKeys
                .Ascending("route.routeNumber");
            workShifts.Indexes.CreateOne(new CreateIndexModel<WorkShiftLog>(routeNumberIndex));
        }
    }
}