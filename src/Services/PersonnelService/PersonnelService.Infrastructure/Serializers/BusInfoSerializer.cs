using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Infrastructure.Serializers
{
    public class BusInfoSerializer : SerializerBase<BusInfoVO>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BusInfoVO value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteString(nameof(BusInfoVO.BusCountryNumber), value.BusCountryNumber);
            context.Writer.WriteString(nameof(BusInfoVO.Brand), value.Brand);
            context.Writer.WriteEndDocument();
        }

        public override BusInfoVO Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var busNumber = context.Reader.ReadString(nameof(BusInfoVO.BusCountryNumber));
            var brand = context.Reader.ReadString(nameof(BusInfoVO.Brand));
            context.Reader.ReadEndDocument();

            return new BusInfoVO(busNumber, brand);
        }
    }
}