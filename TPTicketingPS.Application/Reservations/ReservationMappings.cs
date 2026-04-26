using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Reservations;

internal static class ReservationMappings
{
    public static ReservationDto ToDto(this Reservation reservation, DateTime utcNow)
    {
        var secondsRemaining = (int)Math.Max(
            0, (reservation.ExpiresAt - utcNow).TotalSeconds);

        var items = reservation.Items
            .Select(item => new ReservationItemDto(
                Id: item.Id,
                SeatId: item.SeatId,
                RowIdentifier: item.Seat?.RowIdentifier ?? string.Empty,
                SeatNumber: item.Seat?.SeatNumber ?? 0,
                SectorName: item.Seat?.Sector?.Name ?? string.Empty,
                UnitPrice: item.UnitPrice))
            .ToList()
            .AsReadOnly();

        return new ReservationDto(
            Id: reservation.Id,
            UserId: reservation.UserId,
            Status: reservation.Status.ToString(),
            ReservedAt: reservation.ReservedAt,
            ExpiresAt: reservation.ExpiresAt,
            TotalAmount: reservation.TotalAmount,
            SecondsRemaining: secondsRemaining,
            Items: items);
    }
}