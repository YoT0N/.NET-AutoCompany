using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public EntityNotFoundException(string entityName, object entityId)
            : base($"{entityName} with ID '{entityId}' was not found.")
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public EntityNotFoundException(string message) : base(message)
        {
            EntityName = string.Empty;
            EntityId = 0;
        }

        public EntityNotFoundException(string entityName, object entityId, Exception innerException)
            : base($"{entityName} with ID '{entityId}' was not found.", innerException)
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
