using Microsoft.EntityFrameworkCore;

namespace TPTicketingPS.Application.Common.Concurrency;

/// <summary>
/// Ejecuta una operación que puede fallar por DbUpdateConcurrencyException
/// reintentando hasta N veces antes de propagar.
/// </summary>
public static class OptimisticRetry
{
    /// <summary>
    /// Cantidad máxima de intentos totales (incluyendo el primero).
    /// 3 = un intento original + 2 reintentos.
    /// </summary>
    public const int MaxAttempts = 3;

    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        var attempt = 0;

        while (true)
        {
            attempt++;
            try
            {
                return await operation();
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxAttempts)
            {
                // Pequeña pausa con jitter para evitar que dos requests reintenten
                // exactamente al mismo tiempo y vuelvan a chocar.
                var delayMs = Random.Shared.Next(20, 100);
                await Task.Delay(delayMs, cancellationToken);
            }
        }
    }
}