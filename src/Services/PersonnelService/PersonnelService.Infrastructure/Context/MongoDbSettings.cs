namespace PersonnelService.Infrastructure.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;

        public string PersonnelCollectionName { get; set; }
        public string DocumentCollectionName { get; set; }
        public string ExaminationCollectionName { get; set; }
        public string WorkShiftCollectionName { get; set; }
    }

}