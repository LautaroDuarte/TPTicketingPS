using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Reservations.UseCases.GetReservationById;

public class GetReservationById(IReservationRepository reservationRepository) : IGetReservationById
{
    public async Task<ReservationDto> ExecuteAsync(
        Guid reservationId,
        CancellationToken cancellationToken = default)
    {
        // Cargamos la reserva con sus items, los seats de cada item y el sector de cada seat.
        var reservation = await reservationRepository.GetByIdWithItemsAndSeatsAsync(reservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Reservation), reservationId);

        return reservation.ToDto(DateTime.UtcNow);
    }
}