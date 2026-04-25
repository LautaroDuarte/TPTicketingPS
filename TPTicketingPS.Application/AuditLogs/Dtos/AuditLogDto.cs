namespace Ticketing.Application.AuditLogs.Dtos;

public sealed record AuditLogDto(
    Guid Id,
    int? UserId,
    string Action,
    string EntityType,
    string EntityId,
    string? Details,
    DateTime CreatedAt);

public sealed record AuditLogQueryParameters
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int? UserId { get; init; }
    public string? Action { get; init; }
    public string? EntityType { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}