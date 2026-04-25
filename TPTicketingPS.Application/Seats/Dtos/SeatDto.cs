namespace TPTicketingPS.Application.Seats.Dtos;

public sealed record SeatDto(
    Guid Id,
    int SectorId,
    string SectorName,
    string RowIdentifier,
    int SeatNumber,
    string Status,
    decimal Price);

public sealed record SeatMapDto(
    int EventId,
    string EventName,
    IReadOnlyCollection<SectorSeatsDto> Sectors);

public sealed record SectorSeatsDto(
    int SectorId,
    string SectorName,
    decimal Price,
    IReadOnlyCollection<SeatDto> Seats);