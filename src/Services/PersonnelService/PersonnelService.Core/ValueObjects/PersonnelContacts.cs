using MongoDB.Bson.Serialization.Attributes;
using PersonnelService.Domain.Common;
using PersonnelService.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace PersonnelService.Domain.ValueObjects
{
    public class PersonnelContactsVO : ValueObject
    {
        [BsonElement("phone")]
        public string Phone { get; }

        [BsonElement("email")]
        public string Email { get; }

        [BsonElement("address")]
        public string Address { get; }

        public PersonnelContactsVO(string phone, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone number cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            if (!IsValidEmail(email))
                throw new DomainException($"Invalid email format: {email}");

            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Address cannot be empty");

            Phone = phone;
            Email = email;
            Address = address;
        }

        [BsonConstructor]
        private PersonnelContactsVO() { }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Phone;
            yield return Email.ToLowerInvariant();
            yield return Address;
        }

        private static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }
    }
}