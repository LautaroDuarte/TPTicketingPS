using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Application.Common.Exceptions;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Domain.Entities;
using TPTicketingPS.Domain.Enums;

namespace TPTicketingPS.Application.Reservations.UseCases.GetUserReservations;

public class GetUserReservations(IUserRepository userRepository, IReservationRepository reservationRepository) : IGetUserReservations
{
    public async Task<IReadOnlyCollection<ReservationDto>> ExecuteAsync(
        int userId,
        GetUserReservationsQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        // Validamos que el usuario exista. Si no, 404 explícito.
        var userExists = await userRepository.ExistsAsync(
            u => u.Id == userId, cancellationToken);

        if (!userExists)
            throw new NotFoundException(nameof(User), userId);

        // Parseo y validación
        ReservationStatus? statusFilter = null;
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

            statusFilter = status;
        }

        var reservations = await reservationRepository.GetByUserAsync(userId, statusFilter, cancellationToken);

        var utcNow = DateTime.UtcNow;
        return reservations
            .Select(r => r.ToDto(utcNow))
            .ToList()
            .AsReadOnly();
    }
}