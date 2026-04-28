namespace TPTicketingPS.Application.Reservations.Dtos;

public sealed record ReservationDto(
    Guid Id,
    int UserId,
    string Status,
    DateTime ReservedAt,
    DateTime ExpiresAt,
    decimal TotalAmount,
    int SecondsRemaining,
    IReadOnlyCollection<ReservationItemDto> Items);

public sealed record ReservationItemDto(
    Guid Id,
    Guid SeatId,
    string RowIdentifier,
    int SeatNumber,
    string SectorName,
    decimal UnitPrice);