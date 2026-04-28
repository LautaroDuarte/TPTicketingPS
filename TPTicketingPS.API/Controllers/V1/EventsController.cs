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

        public EventsController(
            IGetEvents getEvents,
            IGetSeats getSeats)
        {
            _getEvents = getEvents;
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

        [HttpPost]

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