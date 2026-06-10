using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Common.Models;
using TPTicketingPS.Application.Events;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Application.Seats;
using TPTicketingPS.Application.Users.Dtos;
using TPTicketingPS.Application.Users.UseCases.CreateUser;

namespace TPTicketingPS.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IGetEvents _getEvents;
        private readonly IGetSeats _getSeats;
        private readonly IGetEventById _getEventById;
        private readonly ICreateEvent _createEvent;

        public EventsController(
            IGetEvents getEvents,
            IGetEventById getEventById,
            IGetSeats getSeats,
            ICreateEvent createEvent)
        {
            _getEvents = getEvents;
            _getEventById = getEventById;
            _getSeats = getSeats;
            _createEvent = createEvent;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<EventSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<EventSummaryDto>>> GetEvents(
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

        /// <summary>
        /// Crea un nuevo evento con sus sectores y butacas. Solo administradores.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> Create(
            [FromBody] CreateEventRequest request,
            CancellationToken cancellationToken)
        {
            var eventId = await _createEvent.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { eventId }, new { id = eventId });
        }
    }
}