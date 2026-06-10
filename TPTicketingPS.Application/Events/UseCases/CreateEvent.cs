using FluentValidation;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Events;

public class CreateEvent(
    IAppDbContext context,
    IEventRepository eventRepository,
    IUserRepository userRepository,
    IValidator<CreateEventRequest> validator,
    ICurrentUser currentUser) : ICreateEvent
{
    public async Task<int> ExecuteAsync(
        CreateEventRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validar formato
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        // 2. Identificar usuario
        var userId = currentUser.UserId
            ?? throw new  Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["X-User-Id"] = new[] { "Falta el header X-User-Id." }
            });

        // 3. Validar que sea admin
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (user.Role != "admin")
            throw new ConflictException("Solo los administradores pueden crear eventos.");

        // 4. Crear el evento con sus sectores y butacas
        var ev = new Event(
            request.Name,
            request.EventDate,
            request.Venue,
            request.Description,
            request.MaxReservationsPerUser ?? 4);

        foreach (var sectorReq in request.Sectors)
        {
            var capacity = sectorReq.Rows * sectorReq.SeatsPerRow;
            var sector = new Sector(
                eventId: 0,           // EF lo setea al guardar (relación padre)
                name: sectorReq.Name,
                price: sectorReq.Price,
                capacity: capacity);

            for (var row = 0; row < sectorReq.Rows; row++)
            {
                var rowLetter = ((char)('A' + row)).ToString();
                for (var seatNum = 1; seatNum <= sectorReq.SeatsPerRow; seatNum++)
                {
                    sector.Seats.Add(new Seat(
                        sectorId: 0,
                        rowIdentifier: rowLetter,
                        seatNumber: seatNum));
                }
            }

            ev.Sectors.Add(sector);
        }

        await eventRepository.AddAsync(ev, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return ev.Id;
    }
}