using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Events;

namespace TPTicketingPS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IGetEvents _getEvents;

        public EventsController(IGetEvents getEvents)
        {
            _getEvents = getEvents;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var result = _getEvents.Execute();
            return Ok(result);
        }
    }
}
