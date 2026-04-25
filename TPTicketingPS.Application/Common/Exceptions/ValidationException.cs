// Para 400

namespace TPTicketingPS.Application.Common.Exceptions;

public class ValidationException(IDictionary<string, string[]> errors)
    : Exception("Hay errores de validación en la petición.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}