namespace TPTicketingPS.Application.Reservations.UseCases.CreateReservation;

public sealed record CreateReservationRequest(
    int EventId,
    IReadOnlyCollection<Guid> SeatIds);