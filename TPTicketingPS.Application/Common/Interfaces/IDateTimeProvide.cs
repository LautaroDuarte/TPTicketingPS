namespace TPTicketingPS.Application.Common.Interfaces;

/// <summary>
/// Abstracción del reloj del sistema para poder testear expiración de reservas sin Thread.Sleep.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}