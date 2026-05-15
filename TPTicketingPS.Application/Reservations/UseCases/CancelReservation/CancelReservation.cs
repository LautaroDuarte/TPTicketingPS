using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Auditing;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Reservations.UseCases.CancelReservation;

/// <summary>
/// Cancela una reserva Pending y libera los asientos inmediatamente.
/// Solo el dueño puede cancelar su propia reserva.
/// </summary>
public class CancelReservation(
    IAppDbContext context,
    ICurrentUser currentUser,
    IAuditLogger auditLogger) : ICancelReservation
{
    public async Task ExecuteAsync(
        Guid reservationId,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException(new Dictionary<string, string[]>
            {
                ["X-User-Id"] = new[] { "Falta el header X-User-Id." }
            });

        // Cargar reserva con items y seats
        var reservation = await context.Reservations
            .Include(r => r.Items)
                .ThenInclude(i => i.Seat!)
            .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Reservation), reservationId);

        // Validaciones
        if (reservation.UserId != userId)
            throw new ConflictException("Esta reserva pertenece a otro usuario.");

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new ConflictException("Esta reserva ya fue cancelada.");

        if (reservation.Status == ReservationStatus.Paid)
            throw new ConflictException("No se puede cancelar una reserva ya pagada.");

        if (reservation.Status == ReservationStatus.Expired)
            throw new ConflictException("La reserva ya expiró.");

        // Transacción ACID: cancelar reserva + liberar asientos + auditar
        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // Cancelar la reserva
            reservation.Cancel();

            // Liberar los asientos
            foreach (var item in reservation.Items)
            {
                if (item.Seat!.Status == SeatStatus.Reserved)
                {
                    item.Seat.Release();
                }
            }

            // Auditar
            auditLogger.Log(
                action: AuditActions.ReserveCancelled,
                entityType: AuditEntityTypes.Reservation,
                entityId: reservation.Id.ToString(),
                userId: userId,
                details: new
                {
                    reservationId = reservation.Id,
                    seatsReleased = reservation.Items.Count
                });

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}