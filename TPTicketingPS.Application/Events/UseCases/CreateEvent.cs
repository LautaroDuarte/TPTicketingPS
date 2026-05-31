using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Events;

public class CreateEvent(IAppDbContext context, IEventRepository eventRepository) : ICreateEvent
{
    public async Task<int> ExecuteAsync(
        CreateEventRequest request,
        CancellationToken cancellationToken = default)
    {
        var ev = new Event(
            request.Name,
            request.EventDate,
            request.Venue,
            request.Description,
            request.MaxReservationsPerUser ?? int.MaxValue
        );

        await eventRepository.AddAsync(ev, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return ev.Id;
    }
}