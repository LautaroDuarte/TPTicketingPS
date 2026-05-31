using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Common.Interfaces;

public interface ISeatRepository : IRepository<Seat>
{
    Task<List<Seat>> GetByIdsWithSectorAsync(
        IEnumerable<Guid> seatIds,
        CancellationToken cancellationToken = default);

    Task<List<Seat>> GetByEventIdAsync(
    int eventId,
    CancellationToken cancellationToken = default);
}