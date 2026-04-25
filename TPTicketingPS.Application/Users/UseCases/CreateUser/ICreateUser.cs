using TPTicketingPS.Application.Users.Dtos;

namespace TPTicketingPS.Application.Users.UseCases.CreateUser;

public interface ICreateUser
{
    Task<UserDto> ExecuteAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}