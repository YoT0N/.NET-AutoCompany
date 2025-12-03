using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PersonnelService.Domain.ValueObjects;

namespace PersonnelService.Infrastructure.Serializers
{
    public class ExaminationMetricsSerializer : SerializerBase<ExaminationMetricsVO>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ExaminationMetricsVO value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteInt32(nameof(ExaminationMetricsVO.Height), value.Height);
            context.Writer.WriteInt32(nameof(ExaminationMetricsVO.Weight), value.Weight);
            context.Writer.WriteString(nameof(ExaminationMetricsVO.BloodPressure), value.BloodPressure);
            context.Writer.WriteString(nameof(ExaminationMetricsVO.Vision), value.Vision);
            context.Writer.WriteEndDocument();
        }

        public override ExaminationMetricsVO Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var height = context.Reader.ReadInt32(nameof(ExaminationMetricsVO.Height));
            var weight = context.Reader.ReadInt32(nameof(ExaminationMetricsVO.Weight));
            var bloodPressure = context.Reader.ReadString(nameof(ExaminationMetricsVO.BloodPressure));
            var vision = context.Reader.ReadString(nameof(ExaminationMetricsVO.Vision));
            context.Reader.ReadEndDocument();

            return new ExaminationMetricsVO(height, weight, bloodPressure, vision);
        }
    }
}