using TPTicketingPS.Application.Users.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Users;

internal static class UserMappings
{
    public static UserDto ToDto(this User user) => new(
        Id: user.Id,
        Name: user.Name,
        Email: user.Email,
        PhoneNumber: user.PhoneNumber,
        IsActive: user.IsActive,
        CreatedAt: user.CreatedAt);
}