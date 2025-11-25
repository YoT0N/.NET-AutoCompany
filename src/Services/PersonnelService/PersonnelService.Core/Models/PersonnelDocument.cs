using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonnelService.Core.Models
{
    public class PersonnelDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("personnelId")]
        public int PersonnelId { get; set; }

        [BsonElement("docType")]
        public string DocType { get; set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("uploadedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("issuedOn")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime IssuedOn { get; set; }

        [BsonElement("validUntil")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime ValidUntil { get; set; }
    }
}