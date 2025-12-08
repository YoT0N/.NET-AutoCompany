namespace PersonnelService.Domain.Exceptions
{
    public class PersonnelNotFoundException : DomainException
    {
        public PersonnelNotFoundException(string message) : base(message) { }

        public PersonnelNotFoundException(int personnelId)
            : base($"Personnel with ID {personnelId} was not found") { }
    }
}