using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;

namespace PersonnelService.Domain.ValueObjects
{
    public class RouteInfoVO : ValueObject
    {
        [BsonElement("routeNumber")]
        public string RouteNumber { get; }

        [BsonElement("distanceKm")]
        public double DistanceKm { get; }

        public RouteInfoVO(string routeNumber, double distanceKm)
        {
            if (string.IsNullOrWhiteSpace(routeNumber))
                throw new DomainException("Route number cannot be empty");

            if (distanceKm <= 0)
                throw new DomainException("Distance must be positive");

            RouteNumber = routeNumber;
            DistanceKm = distanceKm;
        }

        [BsonConstructor]
        private RouteInfoVO() { }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return RouteNumber;
            yield return DistanceKm;
        }
    }
}