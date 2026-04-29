using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Events;
using TPTicketingPS.Application.Events.Dtos;
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
        private readonly IGetEventById _getEventById;

        public EventsController(
            IGetEvents getEvents,
            IGetEventById getEventById,
            IGetSeats getSeats)
        {
            _getEvents = getEvents;
            _getEventById = getEventById;
            _getSeats = getSeats;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(
            [FromQuery] EventQueryParameters parameters,
            CancellationToken cancellationToken)
        {
            var result = await _getEvents.ExecuteAsync(parameters, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{eventId}/seats")]
        public async Task<IActionResult> GetSeats(
        int eventId,
        CancellationToken cancellationToken)
        {
            var result = await _getSeats.ExecuteAsync(eventId, cancellationToken);
            return Ok(result);
        }
        [HttpGet("{eventId:int}")]
        public async Task<ActionResult<EventDto>> GetById(
    int eventId,
    CancellationToken cancellationToken)
        {
            var result = await _getEventById.ExecuteAsync(eventId, cancellationToken);
            return Ok(result);
        }
    }
}