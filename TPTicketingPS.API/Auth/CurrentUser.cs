using TPTicketingPS.Application.Common.Interfaces;

namespace TPTicketingPS.API.Auth;

/// <summary>
/// Lee el id del usuario del header X-User-Id.
/// Mecanismo provisional hasta que se implemente autenticación real.
/// </summary>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private const string UserIdHeader = "X-User-Id";

    public int? UserId
    {
        get
        {
            var headerValue = httpContextAccessor.HttpContext?
                .Request.Headers[UserIdHeader].ToString();

            return int.TryParse(headerValue, out var userId) ? userId : null;
        }
    }

    public bool IsAuthenticated => UserId.HasValue;
}