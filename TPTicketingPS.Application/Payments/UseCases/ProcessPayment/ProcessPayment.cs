using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Auditing;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Payments.Dtos;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Payments.UseCases.ProcessPayment;

/// <summary>
/// Simula el procesamiento de un pago para una reserva. 
/// Incluye validación de formato, validaciones de negocio, manejo de transacciones ACID, y auditoría de intentos, éxitos y fallos.
/// </summary>
public class ProcessPayment(
    IAppDbContext context,
    IValidator<ProcessPaymentRequest> validator,
    ICurrentUser currentUser,
    IReservationRepository reservationRepository,
    IAuditLogger auditLogger) : IProcessPayment
{
    public async Task<PaymentReceiptDto> ExecuteAsync(
        Guid reservationId,
        ProcessPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validar formato del request
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        // 2. Identificar al usuario
        var userId = currentUser.UserId
            ?? throw new Common.Exceptions.ValidationException(new Dictionary<string, string[]>
            {
                ["X-User-Id"] = new[] { "Falta el header X-User-Id." }
            });

        // 3. Auditar el intento (queda registrado aunque el pago falle)
        await auditLogger.LogAndSaveAsync(
            action: AuditActions.PaymentAttempt,
            entityType: AuditEntityTypes.Payment,
            entityId: reservationId.ToString(),
            userId: userId,
            details: new { reservationId, request.PaymentMethod },
            cancellationToken: cancellationToken);

        // 4. Cargar reserva con sus items y los seats relacionados (todo en una query)
        var reservation = await reservationRepository.GetByIdWithItemsAndSeatsAsync(reservationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Reservation), reservationId);

        // 5. Validaciones de negocio
        ValidateReservationCanBePaid(reservation, userId);

        // 6. Procesar pago dentro de una transacción ACID
        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // 6a. Marcar reserva como pagada
            reservation.MarkAsPaid();

            // 6b. Marcar todos los asientos como vendidos
            foreach (var item in reservation.Items)
            {
                item.Seat!.MarkAsSold();
            }

            // 6c. Auditar éxito (mismo SaveChanges)
            auditLogger.Log(
                action: AuditActions.PaymentSuccess,
                entityType: AuditEntityTypes.Payment,
                entityId: reservation.Id.ToString(),
                userId: userId,
                details: new
                {
                    reservationId = reservation.Id,
                    totalAmount = reservation.TotalAmount,
                    paymentMethod = request.PaymentMethod,
                    seatsCount = reservation.Items.Count
                });

            // 6d. Persistir todo y commitear
            await context.SaveChangesAsync(cancellationToken);

            //throw new InvalidOperationException("Forzado para demo");

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Cualquier excepción → rollback completo
            await transaction.RollbackAsync(cancellationToken);

            // Auditar fallo (en una nueva conexión, fuera de la transacción rolleada)
            context.ChangeTracker.Clear();
            await auditLogger.LogAndSaveAsync(
                action: AuditActions.PaymentFailed,
                entityType: AuditEntityTypes.Payment,
                entityId: reservationId.ToString(),
                userId: userId,
                details: new { reservationId, reason = "transaction_failed" },
                cancellationToken: cancellationToken);

            throw;
        }

        // 7. Armar comprobante (datos de tarjeta no se guardan, solo se muestran)
        var last4 = ExtractLast4Digits(request.CardNumber);

        return new PaymentReceiptDto(
            ReservationId: reservation.Id,
            PaidAt: DateTime.UtcNow,
            TotalAmount: reservation.TotalAmount,
            PaymentMethod: request.PaymentMethod,
            CardNumberLast4: last4,
            Seats: reservation.Items
                .Select(item => new PaidSeatDto(
                    SeatId: item.SeatId,
                    SectorName: item.Seat!.Sector!.Name,
                    RowIdentifier: item.Seat.RowIdentifier,
                    SeatNumber: item.Seat.SeatNumber,
                    UnitPrice: item.UnitPrice))
                .ToList()
                .AsReadOnly());
    }

    private static void ValidateReservationCanBePaid(Reservation reservation, int userId)
    {
        if (reservation.UserId != userId)
            throw new ConflictException("Esta reserva pertenece a otro usuario.");

        if (reservation.Status == ReservationStatus.Paid)
            throw new ConflictException("Esta reserva ya fue pagada.");

        if (reservation.Status == ReservationStatus.Expired)
            throw new ConflictException("La reserva expiró. Por favor iniciá una nueva.");

        if (reservation.Status != ReservationStatus.Pending)
            throw new ConflictException($"La reserva está en estado {reservation.Status} y no puede pagarse.");

        if (reservation.IsExpired(DateTime.UtcNow))
            throw new ConflictException("La reserva expiró por timeout. Por favor iniciá una nueva.");

        if (reservation.Items.Count == 0)
            throw new ConflictException("La reserva no tiene items para pagar.");
    }

    private static string ExtractLast4Digits(string cardNumber)
    {
        var digits = new string(cardNumber.Where(char.IsDigit).ToArray());
        return digits.Length >= 4 ? digits[^4..] : "****";
        // El operador [^4..] (range) toma los últimos 4 caracteres (C# 8+).
    }
}