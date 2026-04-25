using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Events;

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
        public IActionResult GetEvents()
        {
            var result = _getEvents.Execute();
            return Ok(result);
        }
    }
}
