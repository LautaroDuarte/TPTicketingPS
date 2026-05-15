namespace TPTicketingPS.Application.Reservations.UseCases.CancelReservation;

public interface ICancelReservation
{
    Task ExecuteAsync(Guid reservationId, CancellationToken cancellationToken = default);
}