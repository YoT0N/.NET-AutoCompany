namespace PersonnelService.Application.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }

        public ConflictException(string message, Exception inner) : base(message, inner) { }

        public ConflictException(string entity, string identifier)
            : base($"{entity} with identifier '{identifier}' already exists.") { }
    }
}