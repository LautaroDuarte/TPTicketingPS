using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Users.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Users.UseCases.CreateUser;

public class CreateUser(
    IAppDbContext context,
    IValidator<CreateUserRequest> validator) : ICreateUser
{
    public async Task<UserDto> ExecuteAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validación de formato
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        // Validación de email unico
        var emailAlreadyUsed = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailAlreadyUsed)
        {
            throw new ConflictException($"Ya existe un usuario registrado con el email '{request.Email}'.");
        }

        // Crear entidad. PasswordHash por ahora se setea con un valor dummy, ya que no se especifica en el request.
        // En un caso real, se debería recibir el password en el request, validarlo y hashearlo antes de crear el usuario.
        var user = new User(
            name: request.Name,
            email: request.Email,
            passwordHash: "not-set",
            phoneNumber: request.PhoneNumber);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}