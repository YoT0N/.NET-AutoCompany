using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonnelService.Domain.Common
{
    public abstract class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("version")]
        public int Version { get; protected set; } = 0;

        protected void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }
}