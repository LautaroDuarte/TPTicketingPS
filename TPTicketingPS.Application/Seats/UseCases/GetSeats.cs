using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Seats.Dtos;

namespace TPTicketingPS.Application.Seats;

public class GetSeats(IAppDbContext context) : IGetSeats
{
    public async Task<SeatMapDto> ExecuteAsync(
        int eventId,
        CancellationToken cancellationToken)
    {
        var eventEntity = await context.Events
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken)
            ?? throw new Exception("Event not found");

        var seats = await context.Seats
            .Include(s => s.Sector)
            .Where(s => s.Sector!.EventId == eventId)
            .ToListAsync(cancellationToken);

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
