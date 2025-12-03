using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Entities;

namespace PersonnelService.Infrastructure
{
    public static class MongoDbMappings
    {
        public static void RegisterClassMaps()
        {
            // ----------------- BaseEntity -----------------
            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id)
                      .SetIdGenerator(StringObjectIdGenerator.Instance)
                      .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    cm.SetIgnoreExtraElements(true);
                });
            }

            // ----------------- Personnel -----------------
            if (!BsonClassMap.IsClassMapRegistered(typeof(Personnel)))
            {
                BsonClassMap.RegisterClassMap<Personnel>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            // ----------------- PersonnelDocument -----------------
            if (!BsonClassMap.IsClassMapRegistered(typeof(PersonnelDocument)))
            {
                BsonClassMap.RegisterClassMap<PersonnelDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            // ----------------- PhysicalExamination -----------------
            if (!BsonClassMap.IsClassMapRegistered(typeof(PhysicalExamination)))
            {
                BsonClassMap.RegisterClassMap<PhysicalExamination>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            // ----------------- WorkShiftLog -----------------
            if (!BsonClassMap.IsClassMapRegistered(typeof(WorkShiftLog)))
            {
                BsonClassMap.RegisterClassMap<WorkShiftLog>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}