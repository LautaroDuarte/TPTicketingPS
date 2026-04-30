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

        // 3. Validar que el usuario exista ANTES de intentar auditar
        //    (la FK del AuditLog requiere un userId válido)
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!user.IsActive)
            throw new ConflictException("El usuario está inactivo.");

        // 4. Recién ahora auditamos el intento, sabiendo que el user existe
        await auditLogger.LogAndSaveAsync(
            action: AuditActions.ReserveAttempt,
            entityType: AuditEntityTypes.Reservation,
            entityId: "pending",
            userId: userId,
            details: new { request.EventId, request.SeatIds },
            cancellationToken: cancellationToken);

        // 5. Validar evento y asientos
        var @event = await context.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), request.EventId);

        var seats = await context.Seats
            .Include(s => s.Sector)
            .Where(s => request.SeatIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        // 6. Validaciones sobre los asientos
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

        if (seats.Any(s => s.Sector!.EventId != request.EventId))
        {
            throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["SeatIds"] = new[] { "Algunos asientos no pertenecen al evento indicado." }
            });
        }

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

        // 7. Respetar MaxReservationsPerUser
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

        // 8. Crear la reserva y agregar items
        var reservation = new Reservation(userId);

        foreach (var seat in seats)
        {
            seat.Reserve(reservation.Id);
            reservation.AddItem(new ReservationItem(reservation.Id, seat.Id, seat.Sector!.Price));
        }

        context.Reservations.Add(reservation);

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

        return reservation.ToDto(DateTime.UtcNow);
    }
}