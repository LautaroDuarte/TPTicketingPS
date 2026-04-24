using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.TotalAmount)
            .HasColumnType("decimal(18,2)");

        // El background job escanea por (Status='Pending' AND ExpiresAt < now).
        // Este índice lo hace O(log n) en vez de full scan.
        builder.HasIndex(r => new { r.Status, r.ExpiresAt });

        builder.HasMany(r => r.Items)
            .WithOne(i => i.Reservation)
            .HasForeignKey(i => i.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
