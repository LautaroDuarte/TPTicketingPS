
namespace TPTicketingPS.Application.Events.Dtos;

public sealed record EventDto(
    int Id,
    string Name,
    DateTime EventDate,
    string Venue,
    string Status,
    string? Description,
    int? MaxReservationsPerUser);

public sealed record EventSummaryDto(
    int Id,
    string Name,
    DateTime EventDate,
    string Venue,
    string Status);

public sealed record CreateEventRequest(
    string Name,
    DateTime EventDate,
    string Venue,
    string? Description,
    int? MaxReservationsPerUser);

public sealed record EventQueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Status { get; init; }
}