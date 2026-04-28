using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Users.Dtos;

namespace TPTicketingPS.Application.Users.UseCases.GetUserById;

public class GetUserById(IAppDbContext context) : IGetUserById
{
    public async Task<UserDto> ExecuteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), userId);

        return user.ToDto();
    }
}