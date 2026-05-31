using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio específico de Reservation. Extiende el genérico y agrega
/// las queries que conocen la forma del agregado (Items, Seats, Sectors).
/// </summary>
public class ReservationRepository(AppDbContext context)
    : Repository<Reservation>(context), IReservationRepository
{
    public async Task<Reservation?> GetByIdWithItemsAndSeatsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await Context.Reservations
            .Include(r => r.Items)
                .ThenInclude(i => i.Seat!)
                .ThenInclude(s => s.Sector!)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<int> CountActiveByUserAndEventAsync(
        int userId,
        int eventId,
        CancellationToken cancellationToken = default)
        => await Context.Reservations
            .Where(r => r.UserId == userId
                && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Paid)
                && r.Items.Any(i => i.Seat!.Sector!.EventId == eventId))
            .CountAsync(cancellationToken);

    public async Task<List<Reservation>> GetExpiredPendingAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default)
        => await Context.Reservations
            .Include(r => r.Items)
                .ThenInclude(i => i.Seat!)
            .Where(r => r.Status == ReservationStatus.Pending && r.ExpiresAt <= utcNow)
            .ToListAsync(cancellationToken);

    public async Task<List<Reservation>> GetByUserAsync(
        int userId,
        ReservationStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Reservations
            .AsNoTracking()
            .Include(r => r.Items)
                .ThenInclude(i => i.Seat!)
                .ThenInclude(s => s.Sector!)
            .Where(r => r.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .OrderByDescending(r => r.ReservedAt)
            .ToListAsync(cancellationToken);
    }
}