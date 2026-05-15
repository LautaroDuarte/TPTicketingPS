using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Auditing;
using TPTicketingPS.Application.Common.Concurrency;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Reservations.UseCases.CreateReservation;

/// <summary>
/// Versión con Optimistic Locking sobre Seat.Version y transacción explícita.
///
/// Si dos requests intentan reservar el mismo asiento al mismo tiempo, EF detecta
/// el conflicto al hacer SaveChanges (la Version cambió entre el read y el write)
/// y lanza DbUpdateConcurrencyException. Reintentamos hasta 2 veces más con jitter;
/// si después de 3 intentos no podemos, devolvemos 409 Conflict.
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

        // 2. Identificar al usuario
        var userId = currentUser.UserId
            ?? throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["X-User-Id"] = new[] { "Falta el header X-User-Id." }
            });

        // 3. Validar que el usuario exista (FK del AuditLog requiere userId válido)
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!user.IsActive)
            throw new ConflictException("El usuario está inactivo.");

        // 4. Auditar el intento (queda registrado aunque falle)
        await auditLogger.LogAndSaveAsync(
            action: AuditActions.ReserveAttempt,
            entityType: AuditEntityTypes.Reservation,
            entityId: "pending",
            userId: userId,
            details: new { request.EventId, request.SeatIds },
            cancellationToken: cancellationToken);

        // 5. Ejecutar la operación crítica con retry ante conflicto de concurrencia
        try
        {
            return await OptimisticRetry.ExecuteAsync(
                () => TryCreateReservationAsync(request, userId, cancellationToken),
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // limpiamos el ChangeTracker para evitar que EF intente seguir guardando entidades con versiones desactualizadas
            context.ChangeTracker.Clear();

            // Agotamos los reintentos: el conflicto persiste.
            // Auditar el fallo y devolver 409.
            await auditLogger.LogAndSaveAsync(
                action: AuditActions.ReserveFailedConcurrency,
                entityType: AuditEntityTypes.Reservation,
                entityId: "n/a",
                userId: userId,
                details: new { request.EventId, request.SeatIds, reason = "concurrency_after_retries" },
                cancellationToken: cancellationToken);

            throw new ConflictException(
                "No pudimos completar la reserva por alta demanda. Intentá nuevamente.");
        }
    }

    /// <summary>
    /// Intento individual de creación de reserva. Cada llamada abre su propia
    /// transacción para que un fallo a mitad de camino no deje datos parciales.
    /// </summary>
    private async Task<ReservationDto> TryCreateReservationAsync(
        CreateReservationRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        // Validar evento
        var @event = await context.Events
            .FirstOrDefaultAsync(e => e.Id == request.EventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), request.EventId);

        // Cargar seats con traking
        // y ordenados por Id para minimizar deadlocks
        var seats = await context.Seats
            .Include(s => s.Sector)
            .Where(s => request.SeatIds.Contains(s.Id))
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);

        // Verificar que existan todos los pedidos
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

        // Pertenecen al evento
        if (seats.Any(s => s.Sector!.EventId != request.EventId))
        {
            throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["SeatIds"] = new[] { "Algunos asientos no pertenecen al evento indicado." }
            });
        }

        // Disponibilidad
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

        // Límite por usuario
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

        // Crear reserva e ítems, cambiar estado de seats
        var reservation = new Reservation(userId);

        foreach (var seat in seats)
        {
            seat.Reserve(reservation.Id);
            reservation.AddItem(new ReservationItem(reservation.Id, seat.Id, seat.Sector!.Price));
        }

        context.Reservations.Add(reservation);

        // Auditar éxito (mismo SaveChanges que la reserva → atómico)
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

        await transaction.CommitAsync(cancellationToken);

        return reservation.ToDto(DateTime.UtcNow);
    }
}