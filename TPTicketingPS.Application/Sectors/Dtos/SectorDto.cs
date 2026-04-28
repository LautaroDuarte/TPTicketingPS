namespace TPTicketingPS.Application.Sectors.Dtos;

public sealed record SectorDto(
    int Id,
    int EventId,
    string Name,
    decimal Price,
    int Capacity,
    int AvailableSeats);