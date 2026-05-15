using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TPTicketingPS.Application.Common.Auditing;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Infrastructure.BackgroundJobs;

/// <summary>
/// Servicio asincrónico que cada N segundos busca reservas en estado Pending
/// que hayan superado su ExpiresAt y las marca como Expired, liberando los seats.
/// </summary>
public class ReservationExpirationJob(
    IServiceScopeFactory scopeFactory,
    ILogger<ReservationExpirationJob> logger) : BackgroundService
{
    // Cada cuántos segundos verifica reservas expiradas.
    // usamos 15s para que sea visible en demos.
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "ReservationExpirationJob iniciado. Intervalo: {Interval}s",
            CheckInterval.TotalSeconds);

        // Bucle principal: corre mientras la app esté viva
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredReservationsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // La app se está cerrando, salida limpia
                break;
            }
            catch (Exception ex)
            {
                // Cualquier otro error: lo logueamos pero no detenemos el job.
                logger.LogError(ex, "Error procesando reservas expiradas.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }

        logger.LogInformation("ReservationExpirationJob detenido.");
    }

    private async Task ProcessExpiredReservationsAsync(CancellationToken cancellationToken)
    {
        // Scope manual para resolver el DbContext (BackgroundService -> Singleton, DbContext -> Scoped).
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var auditLogger = scope.ServiceProvider.GetRequiredService<IAuditLogger>();

        var utcNow = DateTime.UtcNow;

        // Traemos solo lo que necesitamos: reservas Pending que ya pasaron su ExpiresAt
        var expiredReservations = await context.Reservations
            .Include(r => r.Items)
                .ThenInclude(i => i.Seat!)
            .Where(r => r.Status == ReservationStatus.Pending && r.ExpiresAt <= utcNow)
            .ToListAsync(cancellationToken);

        if (expiredReservations.Count == 0)
            return;

        logger.LogInformation(
            "Encontradas {Count} reservas para expirar.",
            expiredReservations.Count);

        // Procesamos cada reserva dentro de una transacción individual.
        // Si una falla, las demás siguen procesándose.
        foreach (var reservation in expiredReservations)
        {
            try
            {
                await ExpireReservationAsync(reservation, context, auditLogger, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error expirando reserva {ReservationId}. Se reintentará en el próximo ciclo.",
                    reservation.Id);

                // Limpiamos el tracker para que el siguiente loop no arrastre el error
                context.ChangeTracker.Clear();
            }
        }
    }

    private async Task ExpireReservationAsync(
        Domain.Entities.Reservation reservation,
        IAppDbContext context,
        IAuditLogger auditLogger,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Marcar reserva como Expired
            reservation.Expire();

            // 2. Liberar todos los asientos asociados
            foreach (var item in reservation.Items)
            {
                if (item.Seat!.Status == SeatStatus.Reserved)
                {
                    item.Seat.Release();
                }
            }

            // 3. Auditar la expiración (UserId=null porque la hace el sistema)
            auditLogger.Log(
                action: AuditActions.ReserveExpired,
                entityType: AuditEntityTypes.Reservation,
                entityId: reservation.Id.ToString(),
                userId: null,
                details: new
                {
                    reservationId = reservation.Id,
                    userId = reservation.UserId,
                    seatsReleased = reservation.Items.Count,
                    expiredAt = reservation.ExpiresAt
                });

            // 4. Commit
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Reserva {ReservationId} expirada. Liberados {Count} asientos.",
                reservation.Id,
                reservation.Items.Count);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}