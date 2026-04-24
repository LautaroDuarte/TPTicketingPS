using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasMany(s => s.Seats)
            .WithOne(seat => seat.Sector)
            .HasForeignKey(seat => seat.SectorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}