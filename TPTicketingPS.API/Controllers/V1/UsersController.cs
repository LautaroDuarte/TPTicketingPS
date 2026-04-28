using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Reservations.Dtos;
using TPTicketingPS.Application.Reservations.UseCases.GetUserReservations;
using TPTicketingPS.Application.Users.Dtos;
using TPTicketingPS.Application.Users.UseCases.CreateUser;
using TPTicketingPS.Application.Users.UseCases.GetUserById;

namespace TPTicketingPS.API.Controllers.V1;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public class UsersController(
    ICreateUser createUser,
    IGetUserById getUserById,
    IGetUserReservations getUserReservations) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await createUser.ExecuteAsync(request, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { userId = user.Id },
            value: user);
    }

    [HttpGet("{userId:int}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(
        [FromRoute] int userId,
        CancellationToken cancellationToken)
    {
        var user = await getUserById.ExecuteAsync(userId, cancellationToken);
        return Ok(user);
    }

    [HttpGet("{userId:int}/reservations")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<ReservationDto>>> GetReservations(
        [FromRoute] int userId,
        [FromQuery] GetUserReservationsQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var reservations = await getUserReservations.ExecuteAsync(userId, parameters, cancellationToken);
        return Ok(reservations);
    }
}