using TPTicketingPS.Application.Users.Dtos;

namespace TPTicketingPS.Application.Users.UseCases.GetUserById;

public interface IGetUserById
{
    Task<UserDto> ExecuteAsync(int userId, CancellationToken cancellationToken = default);
}