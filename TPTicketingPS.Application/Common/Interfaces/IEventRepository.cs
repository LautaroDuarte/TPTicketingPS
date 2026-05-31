using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Common.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<Event?> GetByIdWithSectorsAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<(List<Event> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        CancellationToken cancellationToken = default);
}