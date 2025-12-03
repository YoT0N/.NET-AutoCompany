using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.ValueObjects;
using PersonnelService.Domain.Exceptions;

namespace PersonnelService.Domain.Entities
{
    public class PhysicalExamination : BaseEntity
    {
        [BsonElement("personnelId")]
        public int PersonnelId { get; private set; }

        [BsonElement("examDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ExamDate { get; private set; }

        [BsonElement("result")]
        public string Result { get; private set; } = string.Empty;

        [BsonElement("doctorName")]
        public string DoctorName { get; private set; } = string.Empty;

        [BsonElement("notes")]
        public string? Notes { get; private set; }

        [BsonElement("metrics")]
        public ExaminationMetricsVO Metrics { get; private set; }

        public PhysicalExamination(
            int personnelId,
            DateTime examDate,
            string result,
            string doctorName,
            ExaminationMetricsVO metrics,
            string? notes = null)
        {
            if (personnelId <= 0)
                throw new DomainException("PersonnelId must be positive");

            if (string.IsNullOrWhiteSpace(result))
                throw new DomainException("Examination result cannot be empty");

            if (string.IsNullOrWhiteSpace(doctorName))
                throw new DomainException("Doctor name cannot be empty");

            PersonnelId = personnelId;
            ExamDate = examDate;
            Result = result;
            DoctorName = doctorName;
            Metrics = metrics ?? throw new DomainException("Metrics cannot be null");
            Notes = notes;
        }

        [BsonConstructor]
        private PhysicalExamination() { }

        public void UpdateExamination(
            string result,
            string doctorName,
            ExaminationMetricsVO metrics,
            string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(result))
                throw new DomainException("Examination result cannot be empty");

            if (string.IsNullOrWhiteSpace(doctorName))
                throw new DomainException("Doctor name cannot be empty");

            Result = result;
            DoctorName = doctorName;
            Metrics = metrics ?? throw new DomainException("Metrics cannot be null");
            Notes = notes;
            Touch();
        }

        public bool IsPassed() => Result.Equals("Passed", StringComparison.OrdinalIgnoreCase);

        public bool IsRecent(int daysThreshold = 365) =>
            ExamDate >= DateTime.UtcNow.AddDays(-daysThreshold);
    }
}