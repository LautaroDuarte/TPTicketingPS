using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Events;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Application.Events.UseCases;
using TPTicketingPS.Application.Seats;

namespace TPTicketingPS.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/events")]
    [Produces("application/json")]
    public class EventsController : ControllerBase
    {
        private readonly IGetEvents _getEvents;
        private readonly IGetSeats _getSeats;
        private readonly ICreateEvent _createEvent;

        public EventsController(
            IGetEvents getEvents,
            IGetSeats getSeats,
            ICreateEvent createEvent)
        {
            _getEvents = getEvents;
            _getSeats = getSeats;
            _createEvent = createEvent;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(
            [FromQuery] EventQueryParameters parameters,
            CancellationToken cancellationToken)
        {
            var result = await _getEvents.ExecuteAsync(parameters, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request,
            CancellationToken cancellationToken)
        {
            var id = await _createEvent.ExecuteAsync(request, cancellationToken);
            return Ok(id);
        }

        [HttpGet("{eventId}/seats")]
        public async Task<IActionResult> GetSeats(
        int eventId,
        CancellationToken cancellationToken)
        {
            var result = await _getSeats.ExecuteAsync(eventId, cancellationToken);
            return Ok(result);
        }
    }
}