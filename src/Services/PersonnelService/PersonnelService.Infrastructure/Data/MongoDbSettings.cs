namespace PersonnelService.Infrastructure.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string PersonnelCollectionName { get; set; } = "Personnel";
        public string DocumentsCollectionName { get; set; } = "PersonnelDocuments";
        public string ExaminationsCollectionName { get; set; } = "PhysicalExamination";
        public string WorkShiftLogsCollectionName { get; set; } = "WorkShiftLog";
    }
}