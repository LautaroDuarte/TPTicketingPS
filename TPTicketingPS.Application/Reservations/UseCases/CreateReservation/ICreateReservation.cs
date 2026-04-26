using TPTicketingPS.Application.Reservations.Dtos;

namespace TPTicketingPS.Application.Reservations.UseCases.CreateReservation;

public interface ICreateReservation
{
    Task<ReservationDto> ExecuteAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken = default);
}