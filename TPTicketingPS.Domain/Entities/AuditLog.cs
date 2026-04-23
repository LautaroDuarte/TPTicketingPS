using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }

    public int? UserId { get; private set; }
    public User? User { get; private set; }

    public string Action { get; private set; }

    public string EntityType { get; private set; }

    public string EntityId { get; private set; }

    public string? Details { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public AuditLog(
        string action,
        string entityType,
        string entityId,
        int? userId = null,
        string? details = null)
    {
        Id = Guid.NewGuid();
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        UserId = userId;
        Details = details;
    }

    private AuditLog() { }
}
