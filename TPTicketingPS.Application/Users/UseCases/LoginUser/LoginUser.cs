using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Users.Dtos;

namespace TPTicketingPS.Application.Users.UseCases.LoginUser;

/// <summary>
/// Login simulado: valida email + password contra la DB.
/// </summary>
public class LoginUser(
    IAppDbContext context,
    IValidator<LoginRequest> validator) : ILoginUser
{
    public async Task<UserDto> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || user.PasswordHash != request.Password)
            throw new ConflictException("Email o contraseña incorrectos.");

        if (!user.IsActive)
            throw new ConflictException("El usuario está inactivo.");

        return user.ToDto();
    }
}