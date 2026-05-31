using FluentValidation;
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
    IReservationRepository reservationRepository,
    ISeatRepository seatRepository,
    IUserRepository userRepository,
    IEventRepository eventRepository,
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

        // 3. Validar que el usuario exista 
        var userExists = await userRepository.ExistsAsync(
            u => u.Id == userId, cancellationToken);
        if (!userExists)
            throw new NotFoundException(nameof(User), userId);


        // 4. Auditar el intento (queda registrado aunque falle)
        await auditLogger.LogAndSaveAsync(
            action: AuditActions.ReserveAttempt,
            entityType: AuditEntityTypes.Reservation,
            entityId: request.EventId.ToString(),
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
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
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
        catch (ConflictException)
        {
            context.ChangeTracker.Clear();
            await auditLogger.LogAndSaveAsync(
                action: AuditActions.ReserveFailedUnavailable,
                entityType: AuditEntityTypes.Reservation,
                entityId: request.EventId.ToString(),
                userId: userId,
                details: new { request.EventId, request.SeatIds, reason = "unavailable_after_retries" },
                cancellationToken: cancellationToken);
            throw;
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

        try
        {
            // Cargar seats
            var seats = await seatRepository.GetByIdsWithSectorAsync(request.SeatIds, cancellationToken);

            ValidateSeatsForReservation(seats, request);

            await ValidateMaxReservationsPerUser(userId, request.EventId, seats, cancellationToken);

            var reservation = new Reservation(userId);

            foreach (var seat in seats)
            {
                seat.Reserve(reservation.Id);
                reservation.AddItem(new ReservationItem(reservation.Id, seat.Id, seat.Sector!.Price));
            }

            await reservationRepository.AddAsync(reservation, cancellationToken);

            auditLogger.Log(
                action: AuditActions.ReserveSuccess,
                entityType: AuditEntityTypes.Reservation,
                entityId: reservation.Id.ToString(),
                userId: userId,
                details: new
                {
                    reservationsId = reservation.Id,
                    seatIds = request.SeatIds
                });

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return reservation.ToDto(DateTime.UtcNow);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static void ValidateSeatsForReservation(
        List<Seat> seats,
        CreateReservationRequest request)
    {
        if (seats.Count != request.SeatIds.Count)
            throw new NotFoundException("Uno o más asientos no existen.", seats);

        if (seats.Any(s => s.Sector!.EventId != request.EventId))
            throw new ConflictException("Algún asiento no pertenece al evento indicado.");

        var unavailable = seats.Where(s => s.Status != SeatStatus.Available).ToList();
        if (unavailable.Count > 0)
            throw new ConflictException("Uno o más asientos ya no están disponibles.");
    }

    private async Task ValidateMaxReservationsPerUser(
        int userId,
        int eventId,
        List<Seat> seats,
        CancellationToken cancellationToken)
    {
        var evt = await eventRepository.GetByIdAsync(eventId, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), eventId);

        var activeCount = await reservationRepository.CountActiveByUserAndEventAsync(
            userId, eventId, cancellationToken);

        if (activeCount + seats.Count > evt.MaxReservationsPerUser)
            throw new ConflictException(
                $"Superás el máximo de {evt.MaxReservationsPerUser} reservas para este evento.");
    }
}