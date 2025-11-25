using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonnelService.Core.Models
{
    public class PhysicalExamination
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("personnelId")]
        public int PersonnelId { get; set; }

        [BsonElement("examDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime ExamDate { get; set; }

        [BsonElement("result")]
        public string Result { get; set; } = string.Empty;

        [BsonElement("doctorName")]
        public string DoctorName { get; set; } = string.Empty;

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("metrics")]
        public ExaminationMetrics Metrics { get; set; } = new ExaminationMetrics();
    }

    public class ExaminationMetrics
    {
        [BsonElement("height")]
        public int Height { get; set; }

        [BsonElement("weight")]
        public int Weight { get; set; }

        [BsonElement("bloodPressure")]
        public string BloodPressure { get; set; } = string.Empty;

        [BsonElement("vision")]
        public string Vision { get; set; } = string.Empty;
    }
}