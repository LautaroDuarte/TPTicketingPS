using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(50);
        builder.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Details).HasMaxLength(4000);

        // Para consultas típicas de auditoría ("últimos logs", "logs de un usuario")
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => new { a.UserId, a.CreatedAt });
    }
}