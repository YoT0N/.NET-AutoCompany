using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Infrastructure.Serializers
{
    public class RouteInfoSerializer : SerializerBase<RouteInfoVO>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, RouteInfoVO value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteString(nameof(RouteInfoVO.RouteNumber), value.RouteNumber);
            context.Writer.WriteDouble(nameof(RouteInfoVO.DistanceKm), value.DistanceKm);
            context.Writer.WriteEndDocument();
        }

        public override RouteInfoVO Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var routeNumber = context.Reader.ReadString(nameof(RouteInfoVO.RouteNumber));
            var distanceKm = context.Reader.ReadDouble(nameof(RouteInfoVO.DistanceKm));
            context.Reader.ReadEndDocument();

            return new RouteInfoVO(routeNumber, distanceKm);
        }
    }
}