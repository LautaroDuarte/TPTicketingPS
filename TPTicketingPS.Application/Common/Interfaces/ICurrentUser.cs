namespace TPTicketingPS.Application.Common.Interfaces;

/// <summary>
/// Acceso al usuario que dispara la operación actual.
/// Implementación leerá el header X-User-Id mientras no haya auth real.
/// </summary>
public interface ICurrentUser
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
}