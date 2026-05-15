namespace TPTicketingPS.Application.Users.UseCases.LoginUser;

public sealed record LoginRequest(
    string Email,
    string Password);