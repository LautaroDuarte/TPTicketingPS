using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Events;
public class GetEventById(IEventRepository eventRepository) : IGetEventById
{
    public async Task<EventDto> ExecuteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var ev = await eventRepository.GetByIdAsync(id, cancellationToken)
              ?? throw new NotFoundException(nameof(Event), id);

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