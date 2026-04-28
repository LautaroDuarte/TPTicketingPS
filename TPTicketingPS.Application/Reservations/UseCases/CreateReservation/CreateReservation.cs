using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Auditing;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Reservations.UseCases.CreateReservation;

/// <summary>
/// Versión "naive": sin control estricto de concurrencia.
/// </summary>
public class CreateReservation(
    IAppDbContext context,
    IValidator<CreateReservationRequest> validator,
    ICurrentUser currentUser,
    IAuditLogger auditLogger) : ICreateReservation
{
    public async Task<ReservationDto> ExecuteAsync(
        CreateReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validación de formato
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        // 2. Identificar al usuario (header X-User-Id)
        var userId = currentUser.UserId
            ?? throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["X-User-Id"] = new[] { "Falta el header X-User-Id." }
            });

        // 3. Auditamos el intento antes de procesarlo, así queda registrado aunque falle
        await auditLogger.LogAndSaveAsync(
            action: AuditActions.ReserveAttempt,
            entityType: AuditEntityTypes.Reservation,
            entityId: "pending",
            userId: userId,
            details: new { request.EventId, request.SeatIds },
            cancellationToken: cancellationToken);

        // 4. Validar que existan: usuario, evento, asientos
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!user.IsActive)
            throw new ConflictException("El usuario está inactivo.");

        var @event = await context.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), request.EventId);

        // Traemos los asientos solicitados, junto con su sector (para precio y validación de pertenencia al evento)
        var seats = await context.Seats
            .Include(s => s.Sector)
            .Where(s => request.SeatIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        // 5. Validaciones de negocio sobre los asientos

        // 5a. Que existan todos los IDs solicitados
        if (seats.Count != request.SeatIds.Count)
        {
            var foundIds = seats.Select(s => s.Id).ToHashSet();
            var missing = request.SeatIds.Where(id => !foundIds.Contains(id)).ToList();

            await auditLogger.LogAndSaveAsync(
                action: AuditActions.ReserveFailedUnavailable,
                entityType: AuditEntityTypes.Reservation,
                entityId: "n/a",
                userId: userId,
                details: new { reason = "missing_seats", missing },
                cancellationToken: cancellationToken);

            throw new NotFoundException(nameof(Seat), string.Join(", ", missing));
        }

        // 5b. Que todos pertenezcan al evento solicitado
        if (seats.Any(s => s.Sector!.EventId != request.EventId))
        {
            throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["SeatIds"] = new[] { "Algunos asientos no pertenecen al evento indicado." }
            });
        }

        // 5c. Que estén todos disponibles
        var unavailable = seats.Where(s => s.Status != SeatStatus.Available).ToList();
        if (unavailable.Count > 0)
        {
            await auditLogger.LogAndSaveAsync(
                action: AuditActions.ReserveFailedUnavailable,
                entityType: AuditEntityTypes.Reservation,
                entityId: "n/a",
                userId: userId,
                details: new
                {
                    reason = "unavailable_seats",
                    seats = unavailable.Select(s => new { s.Id, s.Status })
                },
                cancellationToken: cancellationToken);

            throw new ConflictException(
                $"Los siguientes asientos no están disponibles: " +
                string.Join(", ", unavailable.Select(s => s.Id)));
        }

        // 5d. Respetar el límite de reservas por usuario para este evento
        //  cuántos asientos ya tiene reservados/comprados
        var alreadyReserved = await context.Reservations
            .Where(r => r.UserId == userId
                        && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Paid)
                        && r.Items.Any(i => i.Seat!.Sector!.EventId == request.EventId))
            .SelectMany(r => r.Items)
            .CountAsync(cancellationToken);

        if (alreadyReserved + seats.Count > @event.MaxReservationsPerUser)
        {
            throw new ConflictException(
                $"Excede el máximo de {@event.MaxReservationsPerUser} reservas por usuario para este evento.");
        }

        // 6. Crear la reserva y agregar items
        var reservation = new Reservation(userId);

        foreach (var seat in seats)
        {
            seat.Reserve(reservation.Id);   // cambia Status a Reserved + setea CurrentReservationId
            reservation.AddItem(new ReservationItem(reservation.Id, seat.Id, seat.Sector!.Price));
        }

        context.Reservations.Add(reservation);

        // 7. Auditamos el éxito (queda en el ChangeTracker, se persiste con el SaveChanges de abajo)
        auditLogger.Log(
            action: AuditActions.ReserveSuccess,
            entityType: AuditEntityTypes.Reservation,
            entityId: reservation.Id.ToString(),
            userId: userId,
            details: new
            {
                seatIds = seats.Select(s => s.Id),
                totalAmount = reservation.TotalAmount,
                expiresAt = reservation.ExpiresAt
            });

        await context.SaveChangesAsync(cancellationToken);

        // 8. Devolver el DTO. Necesitamos que cargue Sector dentro del Seat para el mapping.
        //    Como ya están en memoria (los traímos con Include), la navegación funciona.
        return reservation.ToDto(DateTime.UtcNow);
    }
}