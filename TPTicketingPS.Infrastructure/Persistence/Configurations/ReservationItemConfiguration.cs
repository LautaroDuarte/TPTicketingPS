using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Configurations;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.UnitPrice)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(i => i.Seat)
            .WithMany()
            .HasForeignKey(i => i.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}