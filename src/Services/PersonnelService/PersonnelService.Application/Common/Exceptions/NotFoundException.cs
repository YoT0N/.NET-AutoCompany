using System;

namespace PersonnelService.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception inner) : base(message, inner) { }

        public NotFoundException(string entity, string identifier)
            : base($"{entity} with identifier '{identifier}' was not found.") { }
    }
}