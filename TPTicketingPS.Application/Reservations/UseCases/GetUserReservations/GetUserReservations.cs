using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Reservations.UseCases.GetUserReservations;

public class GetUserReservations(IAppDbContext context) : IGetUserReservations
{
    public async Task<IReadOnlyCollection<ReservationDto>> ExecuteAsync(
        int userId,
        GetUserReservationsQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        // Validamos que el usuario exista. Si no, 404 explícito.
        var userExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!userExists)
            throw new NotFoundException(nameof(User), userId);

        // Armamos la query de a poco según los filtros recibidos.
        var query = context.Reservations
            .AsNoTracking()
            .Include(r => r.Items)
                .ThenInclude(i => i.Seat!)
                .ThenInclude(s => s.Sector!)
            .Where(r => r.UserId == userId);

        // Filtro por estado si vino. Validamos que el string sea un valor del enum,
        // si no, devolvemos 400 con un mensaje.
        if (!string.IsNullOrWhiteSpace(parameters.Status))
        {
            if (!Enum.TryParse<ReservationStatus>(parameters.Status, ignoreCase: true, out var status))
            {
                var validValues = string.Join(", ", Enum.GetNames<ReservationStatus>());
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["status"] = new[] { $"Estado inválido. Valores permitidos: {validValues}." }
                });
            }

            query = query.Where(r => r.Status == status);
        }

        var reservations = await query
            .OrderByDescending(r => r.ReservedAt)
            .ToListAsync(cancellationToken);

        var utcNow = DateTime.UtcNow;
        return reservations
            .Select(r => r.ToDto(utcNow))
            .ToList()
            .AsReadOnly();
    }
}