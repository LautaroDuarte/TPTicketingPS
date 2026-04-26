using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Application.Reservations.UseCases.CreateReservation;

namespace TPTicketingPS.API.Controllers.V1;

[ApiController]
[Route("api/v1/reservations")]
[Produces("application/json")]
public class ReservationsController(ICreateReservation createReservation) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservationDto>> Create(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var reservation = await createReservation.ExecuteAsync(request, cancellationToken);

        // Por ahora devolvemos 201 sin Location header.
        return StatusCode(StatusCodes.Status201Created, reservation);
    }
}