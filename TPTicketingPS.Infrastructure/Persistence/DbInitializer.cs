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
        await SeedEventsAsync(cancellationToken);
        //await SeedEventWithSectorsAndSeatsAsync(cancellationToken);
        //await SeedSimpleEventsAsync(cancellationToken); // Eventos planos

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

    private async Task SeedEventsAsync(CancellationToken cancellationToken)
    {
        // Cada definición de evento incluye sus sectores con su layout (filas x asientos por fila)
        var eventDefinitions = new List<EventSeed>
        {
            new(
                Name: "Concierto de Rock 2026",
                EventDate: DateTime.UtcNow.AddMonths(2),
                Venue: "Estadio Central",
                Description: "Una noche de rock en vivo con bandas locales e internacionales.",
                MaxReservationsPerUser: 4,
                Sectors: new[]
                {
                    new SectorSeed("Platea VIP", 35_000m, Rows: 5, SeatsPerRow: 10),
                    new SectorSeed("Platea General", 25_000m, Rows: 5, SeatsPerRow: 10),
                    new SectorSeed("Campo", 15_000m, Rows: 5, SeatsPerRow: 10),
                }),

            new(
                Name: "Festival de Jazz",
                EventDate: DateTime.UtcNow.AddMonths(1).AddDays(15),
                Venue: "Teatro Colón",
                Description: "Los mejores intérpretes de jazz del país en una sola noche.",
                MaxReservationsPerUser: 2,
                Sectors: new[]
                {
                    new SectorSeed("Palco", 50_000m, Rows: 3, SeatsPerRow: 8),
                    new SectorSeed("Platea", 30_000m, Rows: 6, SeatsPerRow: 10),
                }),

            new(
                Name: "Stand Up Comedy Night",
                EventDate: DateTime.UtcNow.AddDays(20),
                Venue: "Sala Multiteatro",
                Description: "Una noche para reírte hasta llorar con los mejores comediantes.",
                MaxReservationsPerUser: 6,
                Sectors: new[]
                {
                    new SectorSeed("Preferencial", 18_000m, Rows: 4, SeatsPerRow: 12),
                    new SectorSeed("General", 12_000m, Rows: 6, SeatsPerRow: 12),
                }),

            new(
                Name: "Obra de Teatro: Hamlet",
                EventDate: DateTime.UtcNow.AddMonths(3),
                Venue: "Teatro San Martín",
                Description: "El clásico de Shakespeare, una nueva interpretación.",
                MaxReservationsPerUser: 4,
                Sectors: new[]
                {
                    new SectorSeed("Platea Baja", 22_000m, Rows: 8, SeatsPerRow: 10),
                    new SectorSeed("Platea Alta", 15_000m, Rows: 6, SeatsPerRow: 10),
                }),

            new(
                Name: "Recital de Música Electrónica",
                EventDate: DateTime.UtcNow.AddMonths(2).AddDays(10),
                Venue: "Costanera Sur",
                Description: "DJ sets de artistas internacionales, toda la noche.",
                MaxReservationsPerUser: 4,
                Sectors: new[]
                {
                    new SectorSeed("VIP Lounge", 60_000m, Rows: 2, SeatsPerRow: 10),
                    new SectorSeed("Pista Frente", 30_000m, Rows: 5, SeatsPerRow: 15),
                    new SectorSeed("Pista General", 18_000m, Rows: 5, SeatsPerRow: 15),
                }),

            new(
                Name: "Conferencia Tech 2026",
                EventDate: DateTime.UtcNow.AddMonths(4),
                Venue: "Centro de Convenciones",
                Description: "Las últimas tendencias en IA, cloud y desarrollo de software.",
                MaxReservationsPerUser: 3,
                Sectors: new[]
                {
                    new SectorSeed("Auditorio Principal", 40_000m, Rows: 10, SeatsPerRow: 12),
                }),

            new(
                Name: "Recital de Tango",
                EventDate: DateTime.UtcNow.AddDays(45),
                Venue: "La Trastienda",
                Description: "Un homenaje a los grandes del tango argentino.",
                MaxReservationsPerUser: 4,
                Sectors: new[]
                {
                    new SectorSeed("Mesas Frente", 28_000m, Rows: 4, SeatsPerRow: 6),
                    new SectorSeed("Mesas Generales", 18_000m, Rows: 6, SeatsPerRow: 8),
                }),
        };

        var totalSeats = 0;

        foreach (var def in eventDefinitions)
        {
            var @event = new Event(
                name: def.Name,
                eventDate: def.EventDate,
                venue: def.Venue,
                description: def.Description,
                maxReservationsPerUser: def.MaxReservationsPerUser);

            context.Events.Add(@event);
            await context.SaveChangesAsync(cancellationToken);

            foreach (var sectorDef in def.Sectors)
            {
                var capacity = sectorDef.Rows * sectorDef.SeatsPerRow;
                var sector = new Sector(
                    eventId: @event.Id,
                    name: sectorDef.Name,
                    price: sectorDef.Price,
                    capacity: capacity);

                context.Sectors.Add(sector);
                await context.SaveChangesAsync(cancellationToken);

                var seats = BuildSeatsForSector(sector.Id, sectorDef.Rows, sectorDef.SeatsPerRow);
                await context.Seats.AddRangeAsync(seats, cancellationToken);
                totalSeats += seats.Count;
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation(
            "Seed: {EventCount} eventos creados con un total de {SeatCount} butacas.",
            eventDefinitions.Count, totalSeats);
    }

    /*
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
    }*/
    /*private async Task SeedSimpleEventsAsync(CancellationToken cancellationToken)
    {
        var simpleEvents = Enumerable.Range(1, 14).Select(i =>
            new Event(
                name: $"Evento {i}",
                eventDate: DateTime.UtcNow.AddDays(i),
                venue: $"Lugar {i}",
                description: $"Evento de prueba {i}",
                maxReservationsPerUser: 4
            )
        );

        await context.Events.AddRangeAsync(simpleEvents, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seed: 14 eventos simples creados.");
    }*/

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

    // Tipos auxiliares para definir el seed de forma declarativa
    private sealed record EventSeed(
        string Name,
        DateTime EventDate,
        string Venue,
        string Description,
        int MaxReservationsPerUser,
        IReadOnlyCollection<SectorSeed> Sectors);

    private sealed record SectorSeed(
        string Name,
        decimal Price,
        int Rows,
        int SeatsPerRow);
}