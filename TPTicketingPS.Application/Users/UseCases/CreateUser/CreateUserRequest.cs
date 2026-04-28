namespace TPTicketingPS.Application.Users.UseCases.CreateUser;

public sealed record CreateUserRequest(
    string Name,
    string Email,
    string? PhoneNumber);