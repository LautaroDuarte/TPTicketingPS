using TPTicketingPS.Application.Events.Dtos;

public interface IGetEventById
{
    Task<EventDto> ExecuteAsync(int id, CancellationToken cancellationToken = default);
}