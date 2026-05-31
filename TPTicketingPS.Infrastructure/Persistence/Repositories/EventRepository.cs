using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Repositories;

public class  EventRepository(AppDbContext context) : Repository<Event>(context), IEventRepository
{
    public async Task<Event?> GetByIdWithSectorsAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await Context.Events
            .AsNoTracking()
            .Include(e => e.Sectors)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<(List<Event> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        // Query base: solo eventos activos, ordenados por fecha del evento
        var query = Context.Events
            .AsNoTracking()
            .AsQueryable();

        // filtro condicional
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }

       // Contar el total ANTES de aplicar paginación
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginación: saltear los de páginas anteriores, tomar pageSize
        var items = await query
            .OrderBy(e => e.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
