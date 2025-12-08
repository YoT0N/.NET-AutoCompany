using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;

namespace PersonnelService.Domain.ValueObjects
{
    public class BusInfoVO : ValueObject
    {
        [BsonElement("busCountryNumber")]
        public string BusCountryNumber { get; }

        [BsonElement("brand")]
        public string Brand { get; }

        public BusInfoVO(string busCountryNumber, string brand)
        {
            if (string.IsNullOrWhiteSpace(busCountryNumber))
                throw new DomainException("Bus country number cannot be empty");

            if (string.IsNullOrWhiteSpace(brand))
                throw new DomainException("Bus brand cannot be empty");

            BusCountryNumber = busCountryNumber;
            Brand = brand;
        }

        [BsonConstructor]
        private BusInfoVO() { }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BusCountryNumber;
            yield return Brand;
        }
    }
}