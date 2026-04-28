using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.Application.Events;

public class GetEvents(IAppDbContext context) : IGetEvents
{
    public async Task<List<EventSummaryDto>> ExecuteAsync(
        EventQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = context.Events.AsQueryable();

        // 🔎 Filtro por Status (si viene)
        if (!string.IsNullOrWhiteSpace(parameters.Status))
        {
            query = query.Where(e => e.Status == parameters.Status);
        }

        // 📄 Paginación
        var skip = (parameters.Page - 1) * parameters.PageSize;

        var events = await query
            .OrderBy(e => e.EventDate) // opcional pero recomendable
            .Skip(skip)
            .Take(parameters.PageSize)
            .Select(e => new EventSummaryDto(
                e.Id,
                e.Name,
                e.EventDate,
                e.Venue,
                e.Status
            ))
            .ToListAsync(cancellationToken);

        return events;
    }
}