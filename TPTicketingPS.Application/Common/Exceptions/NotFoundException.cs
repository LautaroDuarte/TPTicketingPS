namespace TPTicketingPS.Application.Common.Exceptions;

/// <summary>
/// Recurso solicitado no existe en la DB. Se traduce a 404 Not Found.
/// </summary>
public class NotFoundException : Exception
{
    public string Resource { get; }
    public object Key { get; }

    public NotFoundException(string resource, object key)
        : base($"{resource} con id '{key}' no fue encontrado.")
    {
        Resource = resource;
        Key = key;
    }
}