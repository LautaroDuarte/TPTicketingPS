namespace TPTicketingPS.Application.Reservations.UseCases.GetUserReservations;

public sealed record GetUserReservationsQueryParameters
{
    // Filtros para la consulta
    public string? Status { get; init; }
}