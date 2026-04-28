namespace TPTicketingPS.Application.Common.Interfaces;

/// <summary>
/// Servicio que escribe entradas inmutables en AuditLog.
/// Lo usan los use cases que necesitan dejar rastro de operaciones críticas
/// (reservas, pagos, expiraciones automáticas).
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// Agrega un registro al ChangeTracker pero NO llama a SaveChanges.
    /// El use case decide cuándo persistir, así puede agruparlo en su propia transacción.
    /// </summary>
    void Log(
        string action,
        string entityType,
        string entityId,
        int? userId = null,
        object? details = null);

    /// <summary>
    /// Variante que persiste de inmediato. Útil para auditorías que deben
    /// quedar registradas aunque la operación principal falle (ej. RESERVE_ATTEMPT).
    /// </summary>
    Task LogAndSaveAsync(
        string action,
        string entityType,
        string entityId,
        int? userId = null,
        object? details = null,
        CancellationToken cancellationToken = default);
}