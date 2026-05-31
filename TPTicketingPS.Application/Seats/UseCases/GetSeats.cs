using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Seats.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Seats;

public class GetSeats(ISeatRepository seatRepository, IEventRepository eventRepository) : IGetSeats
{
    public async Task<SeatMapDto> ExecuteAsync(
        int eventId,
        CancellationToken cancellationToken)
    {

        var eventEntity = await eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), eventId);

        var seats = await seatRepository.GetByEventIdAsync(eventId, cancellationToken);

        var grouped = seats
            .GroupBy(s => s.Sector!)
            .Select(g => new SectorSeatsDto(
                g.Key.Id,
                g.Key.Name,
                g.Key.Price,
                g.Select(s => new SeatDto(
                    s.Id,
                    s.SectorId,
                    s.Sector!.Name,
                    s.RowIdentifier,
                    s.SeatNumber,
                    s.Status.ToString(),
                    s.Sector!.Price
                )).ToList()
            ))
            .ToList();

        return new SeatMapDto(
            eventEntity.Id,
            eventEntity.Name,
            grouped
        );
    }
}
