using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Infrastructure.Serializers
{
    public class PersonnelContactsSerializer : SerializerBase<PersonnelContactsVO>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, PersonnelContactsVO value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteString(nameof(PersonnelContactsVO.Phone), value.Phone);
            context.Writer.WriteString(nameof(PersonnelContactsVO.Email), value.Email);
            context.Writer.WriteString(nameof(PersonnelContactsVO.Address), value.Address);
            context.Writer.WriteEndDocument();
        }

        public override PersonnelContactsVO Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var phone = context.Reader.ReadString(nameof(PersonnelContactsVO.Phone));
            var email = context.Reader.ReadString(nameof(PersonnelContactsVO.Email));
            var address = context.Reader.ReadString(nameof(PersonnelContactsVO.Address));
            context.Reader.ReadEndDocument();

            return new PersonnelContactsVO(phone, email, address);
        }
    }
}
