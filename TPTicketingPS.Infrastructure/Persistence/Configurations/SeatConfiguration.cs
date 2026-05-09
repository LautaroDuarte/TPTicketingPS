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

        // Convertimos el enum SeatStatus a string para facilitar la lectura en la base de datos y evitar problemas de compatibilidad.
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Usamos un campo de versión para manejar concurrencia optimista. Esto es crucial para evitar conflictos al reservar asientos.
        builder.Property(s => s.Version)
            .IsRowVersion();

        // Índice para el mapa de asientos y para el job de expiración.
        builder.HasIndex(s => new { s.SectorId, s.Status });

        // Un asiento único por (Sector, Fila, Número)
        builder.HasIndex(s => new { s.SectorId, s.RowIdentifier, s.SeatNumber })
            .IsUnique();
    }
}
