using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonnelService.Core.Models
{
    public class WorkShiftLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("personnelId")]
        public int PersonnelId { get; set; }

        [BsonElement("shiftDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.String)]
        public DateTime ShiftDate { get; set; }

        [BsonElement("startTime")]
        public string StartTime { get; set; } = string.Empty;

        [BsonElement("endTime")]
        public string EndTime { get; set; } = string.Empty;

        [BsonElement("bus")]
        public BusInfo Bus { get; set; } = new BusInfo();

        [BsonElement("route")]
        public RouteInfo Route { get; set; } = new RouteInfo();

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class BusInfo
    {
        [BsonElement("busCountryNumber")]
        public string BusCountryNumber { get; set; } = string.Empty;

        [BsonElement("brand")]
        public string Brand { get; set; } = string.Empty;
    }

    public class RouteInfo
    {
        [BsonElement("routeNumber")]
        public string RouteNumber { get; set; } = string.Empty;

        [BsonElement("distanceKm")]
        public double DistanceKm { get; set; }
    }
}