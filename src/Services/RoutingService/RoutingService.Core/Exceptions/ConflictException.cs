using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.Domain.Exceptions
{
    public class ConflictException : Exception
    {
        public string Resource { get; }
        public string ConflictReason { get; }

        public ConflictException(string message)
            : base(message)
        {
            Resource = string.Empty;
            ConflictReason = message;
        }

        public ConflictException(string resource, string conflictReason)
            : base($"Conflict occurred with {resource}: {conflictReason}")
        {
            Resource = resource;
            ConflictReason = conflictReason;
        }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
            Resource = string.Empty;
            ConflictReason = message;
        }
    }
}
