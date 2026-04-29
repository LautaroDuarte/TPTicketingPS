using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.Application.Events;
public class GetEventById(IAppDbContext context) : IGetEventById
{
    public async Task<EventDto> ExecuteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var ev = await context.Events
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (ev is null)
            throw new NotFoundException("Event", id);

        return new EventDto(
            ev.Id,
            ev.Name,
            ev.EventDate,
            ev.Venue,
            ev.Status,
            ev.Description,
            ev.MaxReservationsPerUser
        );
    }
}