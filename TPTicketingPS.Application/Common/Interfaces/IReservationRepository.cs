using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Common.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{

    Task<Reservation?> GetByIdWithItemsAndSeatsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveByUserAndEventAsync(
        int userId,
        int eventId,
        CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetExpiredPendingAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default);

    Task<List<Reservation>> GetByUserAsync(
        int userId,
        ReservationStatus? status = null,
        CancellationToken cancellationToken = default);
}