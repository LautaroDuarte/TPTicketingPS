using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Events;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/events")]
    [Produces("application/json")]
    public class EventsController : ControllerBase
    {
        private readonly IGetEvents _getEvents;

        public EventsController(IGetEvents getEvents)
        {
            _getEvents = getEvents;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents([FromQuery] EventQueryParameters parameters)
        {
            var result = await _getEvents.ExecuteAsync(parameters);
            return Ok(result);
        }
    }
}