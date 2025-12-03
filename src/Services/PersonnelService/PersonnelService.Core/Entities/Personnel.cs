using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Core.DTOs;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Domain.Entities
{
    public class Personnel : BaseEntity
    {
        [BsonElement("personnelId")]
        public int PersonnelId { get; private set; }

        [BsonElement("fullName")]
        public string FullName { get; private set; } = string.Empty;

        [BsonElement("birthDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime BirthDate { get; private set; }

        [BsonElement("position")]
        public string Position { get; private set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; private set; } = "Active";

        [BsonElement("contacts")]
        public PersonnelContactsVO Contacts { get; private set; }

        private readonly List<PersonnelDocumentInfo> _documents = new();

        [BsonElement("documents")]
        public IReadOnlyCollection<PersonnelDocumentInfo> Documents => _documents.AsReadOnly();

        // Конструктор для створення нового персоналу
        public Personnel(
            int personnelId,
            string fullName,
            DateTime birthDate,
            string position,
            PersonnelContactsVO contacts)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name cannot be empty");

            if (birthDate > DateTime.UtcNow.AddYears(-18))
                throw new DomainException("Personnel must be at least 18 years old");

            if (string.IsNullOrWhiteSpace(position))
                throw new DomainException("Position cannot be empty");

            PersonnelId = personnelId;
            FullName = fullName;
            BirthDate = birthDate;
            Position = position;
            Contacts = contacts ?? throw new DomainException("Contacts cannot be null");
            Status = "Active";
        }

        // Конструктор для MongoDB десеріалізації
        [BsonConstructor]
        private Personnel() { }

        public void UpdatePersonalInfo(string fullName, DateTime birthDate, string position)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name cannot be empty");

            if (string.IsNullOrWhiteSpace(position))
                throw new DomainException("Position cannot be empty");

            FullName = fullName;
            BirthDate = birthDate;
            Position = position;
            Touch();
        }

        public void UpdateContacts(PersonnelContactsVO contacts)
        {
            Contacts = contacts ?? throw new DomainException("Contacts cannot be null");
            Touch();
        }

        public void UpdateStatus(string status)
        {
            var validStatuses = new[] { "Active", "Inactive", "OnLeave", "Terminated" };
            if (!validStatuses.Contains(status))
                throw new InvalidPersonnelStatusException($"Invalid status: {status}");

            Status = status;
            Touch();
        }

        public void AddDocument(PersonnelDocumentInfo document)
        {
            if (document == null)
                throw new DomainException("Document cannot be null");

            _documents.Add(document);
            Touch();
        }

        public void RemoveDocument(string documentType)
        {
            var document = _documents.FirstOrDefault(d => d.Type == documentType);
            if (document != null)
            {
                _documents.Remove(document);
                Touch();
            }
        }

        public bool IsActive() => Status == "Active";

        public int GetAge()
        {
            var today = DateTime.UtcNow;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public class PersonnelDocumentInfo
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("number")]
        public string? Number { get; set; }

        [BsonElement("issuedBy")]
        public string? IssuedBy { get; set; }

        [BsonElement("issuedOn")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? IssuedOn { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("validUntil")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ValidUntil { get; set; }

        public PersonnelDocumentInfo(string type, string? number = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new DomainException("Document type cannot be empty");

            Type = type;
            Number = number;
        }

        [BsonConstructor]
        private PersonnelDocumentInfo() { }

        public bool IsExpired() => ValidUntil.HasValue && ValidUntil.Value < DateTime.UtcNow;

        public bool IsExpiringSoon(int daysThreshold = 30)
        {
            return ValidUntil.HasValue &&
                   ValidUntil.Value > DateTime.UtcNow &&
                   ValidUntil.Value <= DateTime.UtcNow.AddDays(daysThreshold);
        }
    }
}