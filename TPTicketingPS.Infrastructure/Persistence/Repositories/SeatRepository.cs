using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Repositories;

public class SeatRepository(AppDbContext context)
    : Repository<Seat>(context), ISeatRepository
{
    public async Task<List<Seat>> GetByIdsWithSectorAsync(
        IEnumerable<Guid> seatIds,
        CancellationToken cancellationToken = default)
        => await Context.Seats
            .Include(s => s.Sector!)
            .Where(s => seatIds.Contains(s.Id))
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);

    public async Task<List<Seat>> GetByEventIdAsync(
    int eventId,
    CancellationToken cancellationToken = default)
    => await Context.Seats
        .Include(s => s.Sector!)
        .Where(s => s.Sector!.EventId == eventId)
        .ToListAsync(cancellationToken);
}