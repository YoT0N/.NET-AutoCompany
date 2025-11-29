namespace TechnicalService.Domain.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Одна або більше помилок валідації")
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : this(new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        })
    {
    }
}