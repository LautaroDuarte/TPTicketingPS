using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Application.Reservations.UseCases.CreateReservation;
using TPTicketingPS.Application.Reservations.UseCases.GetReservationById;

namespace TPTicketingPS.API.Controllers.V1;

[ApiController]
[Route("api/v1/reservations")]
[Produces("application/json")]
public class ReservationsController(
    ICreateReservation createReservation,
    IGetReservationById getReservationById) : ControllerBase
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

        return CreatedAtRoute(
            routeName: "GetReservationById",
            routeValues: new { reservationId = reservation.Id },
            value: reservation);
    }

    [HttpGet("{reservationId:guid}", Name = "GetReservationById")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> GetById(
        [FromRoute] Guid reservationId,
        CancellationToken cancellationToken)
    {
        var reservation = await getReservationById.ExecuteAsync(reservationId, cancellationToken);
        return Ok(reservation);
    }
}