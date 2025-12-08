namespace PersonnelService.Domain.Exceptions
{
    public class InvalidPersonnelStatusException : DomainException
    {
        public InvalidPersonnelStatusException(string message) : base(message) { }
    }
}