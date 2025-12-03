using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;

namespace PersonnelService.Domain.ValueObjects
{
    public class ExaminationMetricsVO : ValueObject
    {
        [BsonElement("height")]
        public int Height { get; }

        [BsonElement("weight")]
        public int Weight { get; }

        [BsonElement("bloodPressure")]
        public string BloodPressure { get; }

        [BsonElement("vision")]
        public string Vision { get; }

        public ExaminationMetricsVO(int height, int weight, string bloodPressure, string vision)
        {
            if (height <= 0 || height > 250)
                throw new DomainException("Invalid height value");

            if (weight <= 0 || weight > 300)
                throw new DomainException("Invalid weight value");

            if (string.IsNullOrWhiteSpace(bloodPressure))
                throw new DomainException("Blood pressure cannot be empty");

            if (string.IsNullOrWhiteSpace(vision))
                throw new DomainException("Vision cannot be empty");

            Height = height;
            Weight = weight;
            BloodPressure = bloodPressure;
            Vision = vision;
        }

        [BsonConstructor]
        private ExaminationMetricsVO() { }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Height;
            yield return Weight;
            yield return BloodPressure;
            yield return Vision;
        }

        public double CalculateBMI() => Weight / Math.Pow(Height / 100.0, 2);

        public string GetBMICategory()
        {
            var bmi = CalculateBMI();
            return bmi switch
            {
                < 18.5 => "Underweight",
                < 25 => "Normal",
                < 30 => "Overweight",
                _ => "Obese"
            };
        }
    }
}