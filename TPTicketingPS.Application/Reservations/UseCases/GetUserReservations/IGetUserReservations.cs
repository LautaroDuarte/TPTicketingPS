using TPTicketingPS.Application.Reservations.Dtos;

namespace TPTicketingPS.Application.Reservations.UseCases.GetUserReservations;

public interface IGetUserReservations
{
    Task<IReadOnlyCollection<ReservationDto>> ExecuteAsync(
        int userId,
        GetUserReservationsQueryParameters parameters,
        CancellationToken cancellationToken = default);
}