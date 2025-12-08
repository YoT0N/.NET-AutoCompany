using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;

namespace PersonnelService.Domain.Entities
{
    public class PersonnelDocument : BaseEntity
    {
        [BsonElement("personnelId")]
        public int PersonnelId { get; private set; }

        [BsonElement("docType")]
        public string DocType { get; private set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; private set; } = string.Empty;

        [BsonElement("fileSize")]
        public long FileSize { get; private set; }

        [BsonElement("mimeType")]
        public string MimeType { get; private set; } = string.Empty;

        [BsonElement("uploadedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UploadedAt { get; private set; }

        [BsonElement("issuedOn")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? IssuedOn { get; private set; }

        [BsonElement("validUntil")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ValidUntil { get; private set; }

        [BsonElement("category")]
        public string? Category { get; private set; }

        [BsonElement("issuedBy")]
        public string? IssuedBy { get; private set; }

        [BsonElement("documentNumber")]
        public string? DocumentNumber { get; private set; }

        public PersonnelDocument(
            int personnelId,
            string docType,
            string fileName,
            long fileSize,
            string mimeType,
            DateTime? issuedOn = null,
            DateTime? validUntil = null)
        {
            if (personnelId <= 0)
                throw new DomainException("PersonnelId must be positive");

            if (string.IsNullOrWhiteSpace(docType))
                throw new DomainException("Document type cannot be empty");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("File name cannot be empty");

            if (fileSize <= 0)
                throw new DomainException("File size must be positive");

            if (string.IsNullOrWhiteSpace(mimeType))
                throw new DomainException("MIME type cannot be empty");

            PersonnelId = personnelId;
            DocType = docType;
            FileName = fileName;
            FileSize = fileSize;
            MimeType = mimeType;
            UploadedAt = DateTime.UtcNow;
            IssuedOn = issuedOn;
            ValidUntil = validUntil;
        }

        [BsonConstructor]
        private PersonnelDocument() { }

        public void UpdateDocumentDetails(
            DateTime? issuedOn,
            DateTime? validUntil,
            string? category = null,
            string? issuedBy = null,
            string? documentNumber = null)
        {
            IssuedOn = issuedOn;
            ValidUntil = validUntil;
            Category = category;
            IssuedBy = issuedBy;
            DocumentNumber = documentNumber;
            Touch();
        }

        public void ExtendValidity(DateTime newValidUntil)
        {
            if (newValidUntil <= DateTime.UtcNow)
                throw new DomainException("New validity date must be in the future");

            if (ValidUntil.HasValue && newValidUntil <= ValidUntil.Value)
                throw new DomainException("New validity date must be later than current validity");

            ValidUntil = newValidUntil;
            Touch();
        }

        public bool IsExpired() => ValidUntil.HasValue && ValidUntil.Value < DateTime.UtcNow;

        public bool IsExpiringSoon(int daysThreshold = 30)
        {
            return ValidUntil.HasValue &&
                   ValidUntil.Value > DateTime.UtcNow &&
                   ValidUntil.Value <= DateTime.UtcNow.AddDays(daysThreshold);
        }

        public bool IsValid() => !IsExpired();

        public int DaysUntilExpiration()
        {
            if (!ValidUntil.HasValue || IsExpired())
                return 0;

            return (ValidUntil.Value - DateTime.UtcNow).Days;
        }
    }
}