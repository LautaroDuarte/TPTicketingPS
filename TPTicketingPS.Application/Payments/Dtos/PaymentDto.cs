namespace TPTicketingPS.Application.Payments.Dtos;

public sealed record PaymentReceiptDto(
    Guid ReservationId,
    DateTime PaidAt,
    decimal TotalAmount,
    string PaymentMethod,
    string CardNumberLast4,
    IReadOnlyCollection<PaidSeatDto> Seats);

public sealed record PaidSeatDto(
    Guid SeatId,
    string SectorName,
    string RowIdentifier,
    int SeatNumber,
    decimal UnitPrice);