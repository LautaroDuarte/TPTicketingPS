using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Users.Dtos;

namespace TPTicketingPS.Application.Users.UseCases.GetUserById;

public class GetUserById(IUserRepository userRepository) : IGetUserById
{
    public async Task<UserDto> ExecuteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), userId);

        return user.ToDto();
    }
}