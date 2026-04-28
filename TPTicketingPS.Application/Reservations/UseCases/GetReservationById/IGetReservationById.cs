using TPTicketingPS.Application.Reservations.Dtos;

namespace TPTicketingPS.Application.Reservations.UseCases.GetReservationById;

public interface IGetReservationById
{
    Task<ReservationDto> ExecuteAsync(
        Guid reservationId,
        CancellationToken cancellationToken = default);
}