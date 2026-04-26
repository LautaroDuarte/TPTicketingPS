using System.Text.Json;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Infrastructure.Persistence;

namespace TPTicketingPS.Infrastructure.Auditing;

public class AuditLogger(AppDbContext context) : IAuditLogger
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void Log(
        string action,
        string entityType,
        string entityId,
        int? userId = null,
        object? details = null)
    {
        var auditLog = BuildAuditLog(action, entityType, entityId, userId, details);
        context.AuditLogs.Add(auditLog);
    }

    public async Task LogAndSaveAsync(
        string action,
        string entityType,
        string entityId,
        int? userId = null,
        object? details = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = BuildAuditLog(action, entityType, entityId, userId, details);
        context.AuditLogs.Add(auditLog);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static AuditLog BuildAuditLog(
        string action,
        string entityType,
        string entityId,
        int? userId,
        object? details)
    {
        // Si vinieron details, los serializamos a JSON para guardar como metadata estructurada.
        // Esto permite que más adelante una UI de auditoría pueda parsear y mostrarlos.
        var detailsJson = details is null
            ? null
            : JsonSerializer.Serialize(details, JsonOptions);

        return new AuditLog(
            action: action,
            entityType: entityType,
            entityId: entityId,
            userId: userId,
            details: detailsJson);
    }
}