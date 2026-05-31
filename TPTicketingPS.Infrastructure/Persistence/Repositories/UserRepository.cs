using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context)
    : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        => await Context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        => await Context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);
}