namespace TechnicalService.Domain.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityId { get; }

    public NotFoundException(string entityName, object entityId)
        : base($"{entityName} з ідентифікатором '{entityId}' не знайдено")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string message) : base(message)
    {
    }
}