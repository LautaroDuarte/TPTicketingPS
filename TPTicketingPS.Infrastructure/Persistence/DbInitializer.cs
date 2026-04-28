using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Infrastructure.Persistence;

/// <summary>
/// Precarga inicial de la base de datos.
/// Idempotente: si ya hay eventos cargados, no hace nada.
/// </summary>
public class DbInitializer(AppDbContext context, ILogger<DbInitializer> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Si la base no existe, se crea y se aplican todas las migraciones.
      

        if (await context.Events.AnyAsync(cancellationToken))
        {
            logger.LogInformation("La base ya contiene eventos. Se omite el seed.");
            return;
        }

        logger.LogInformation("Iniciando seed de datos de ejemplo...");

        await SeedUsersAsync(cancellationToken);
        await SeedEventWithSectorsAndSeatsAsync(cancellationToken);

        logger.LogInformation("Seed completado.");
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        var users = new[]
        {
            new User(
                name: "Usuario Demo",
                email: "demo@ticketing.local",
                passwordHash: "demo-hash",
                phoneNumber: "+54 11 0000-0000"),
            new User(
                name: "Admin Demo",
                email: "admin@ticketing.local",
                passwordHash: "admin-hash")
        };

        await context.Users.AddRangeAsync(users, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedEventWithSectorsAndSeatsAsync(CancellationToken cancellationToken)
    {
        // 1. Crear el evento
        var rockConcert = new Event(
            name: "Concierto de Rock 2026",
            eventDate: DateTime.UtcNow.AddMonths(2),
            venue: "Estadio Central",
            description: "Evento de demostración para el sistema de ticketing.",
            maxReservationsPerUser: 4);

        context.Events.Add(rockConcert);
        await context.SaveChangesAsync(cancellationToken);

        // 2. Crear los sectores
        var platea = new Sector(
            eventId: rockConcert.Id,
            name: "Platea",
            price: 25_000m,
            capacity: 50);

        var campo = new Sector(
            eventId: rockConcert.Id,
            name: "Campo",
            price: 15_000m,
            capacity: 50);

        context.Sectors.AddRange(platea, campo);
        await context.SaveChangesAsync(cancellationToken);

        // 3. Crear las 50 butacas por sector (5 filas x 10 asientos)
        var plateaSeats = BuildSeatsForSector(platea.Id, rows: 5, seatsPerRow: 10);
        var campoSeats = BuildSeatsForSector(campo.Id, rows: 5, seatsPerRow: 10);

        await context.Seats.AddRangeAsync(plateaSeats, cancellationToken);
        await context.Seats.AddRangeAsync(campoSeats, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Seed: 1 evento '{Event}', 2 sectores ({S1}/{S2}), {Total} butacas creadas.",
            rockConcert.Name, platea.Name, campo.Name, plateaSeats.Count + campoSeats.Count);
    }

    /// <summary>
    /// Genera butacas con identificador de fila A, B, C... y número 1..N.
    /// Total = rows * seatsPerRow.
    /// </summary>
    private static List<Seat> BuildSeatsForSector(int sectorId, int rows, int seatsPerRow)
    {
        var seats = new List<Seat>(capacity: rows * seatsPerRow);

        for (var row = 0; row < rows; row++)
        {
            var rowIdentifier = ((char)('A' + row)).ToString();

            for (var seatNumber = 1; seatNumber <= seatsPerRow; seatNumber++)
            {
                seats.Add(new Seat(sectorId, rowIdentifier, seatNumber));
            }
        }

        return seats;
    }
}