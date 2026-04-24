using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.RowIdentifier)
            .IsRequired()
            .HasMaxLength(10);

        // Guardamos el enum como string en la DB:
        // - Queries legibles ("SELECT * WHERE Status = 'Available'")
        // - Agregar un nuevo estado no rompe datos existentes
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Optimistic Locking: cada UPDATE revisa Version.
        // Si cambió entre el read y el write, EF tira DbUpdateConcurrencyException
        // y lo traducimos a 409 Conflict.
        builder.Property(s => s.Version)
            .IsConcurrencyToken();

        // Índice para el mapa de asientos y para el job de expiración.
        builder.HasIndex(s => new { s.SectorId, s.Status });

        // Un asiento único por (Sector, Fila, Número)
        builder.HasIndex(s => new { s.SectorId, s.RowIdentifier, s.SeatNumber })
            .IsUnique();
    }
}
