namespace TechnicalService.Domain.Exceptions;

/// <summary>
/// Виняток валідації з підтримкою як простих повідомлень, так і структурованих помилок
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Конструктор для словника помилок (для FluentValidation)
    /// </summary>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("Виникли одна або більше помилок валідації")
    {
        Errors = errors;
    }

    /// <summary>
    /// Конструктор для однієї помилки з вказівкою поля
    /// </summary>
    public ValidationException(string propertyName, string errorMessage)
        : base(errorMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
    }

    /// <summary>
    /// Конструктор для простого повідомлення без прив'язки до поля
    /// </summary>
    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { "General", new[] { message } }
        };
    }
}