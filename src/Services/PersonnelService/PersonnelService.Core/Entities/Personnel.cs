using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonnelService.Core.Models
{
    public class Personnel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("personnelId")]
        public int PersonnelId { get; set; }

        [BsonElement("fullName")]
        public string FullName { get; set; } = string.Empty;

        [BsonElement("birthDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime BirthDate { get; set; }

        [BsonElement("position")]
        public string Position { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "Active";

        [BsonElement("contacts")]
        public PersonnelContacts Contacts { get; set; } = new PersonnelContacts();

        [BsonElement("documents")]
        public List<PersonnelDocumentInfo> Documents { get; set; } = new List<PersonnelDocumentInfo>();

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PersonnelContacts
    {
        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;
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
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime? IssuedOn { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("validUntil")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime? ValidUntil { get; set; }
    }
}