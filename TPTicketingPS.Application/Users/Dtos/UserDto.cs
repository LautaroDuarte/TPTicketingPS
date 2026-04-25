namespace TPTicketingPS.Application.Users.Dtos;

public sealed record UserDto(
    int Id,
    string Name,
    string Email,
    string? PhoneNumber,
    bool IsActive,
    DateTime CreatedAt);

public sealed record CreateUserRequest(
    string Name,
    string Email,
    string? PhoneNumber);