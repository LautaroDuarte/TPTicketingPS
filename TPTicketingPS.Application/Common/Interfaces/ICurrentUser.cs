namespace TPTicketingPS.Application.Common.Interfaces;

/// <summary>
/// Lee el id del usuario del header X-User-Id.
/// Centralizamos acá por ahora
/// </summary>
public interface ICurrentUser
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
}