using TPTicketingPS.Application.Users.Dtos;

namespace TPTicketingPS.Application.Users.UseCases.LoginUser;

public interface ILoginUser
{
    Task<UserDto> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default);
}