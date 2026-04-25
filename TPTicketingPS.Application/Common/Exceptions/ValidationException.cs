namespace TPTicketingPS.Application.Common.Exceptions;

/// <summary>
/// Errores de validación con detalle por campo. Se traduce a 400 Bad Request.
/// FluentValidation ya tira la suya (FluentValidation.ValidationException),
/// la nuestra es para validaciones manuales en use cases.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Hay errores de validación en la petición.")
    {
        Errors = errors;
    }
}