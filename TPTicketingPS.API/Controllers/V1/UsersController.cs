using Microsoft.AspNetCore.Mvc;
using TPTicketingPS.Application.Users.Dtos;
using TPTicketingPS.Application.Users.UseCases.CreateUser;
using TPTicketingPS.Application.Users.UseCases.GetUserById;

namespace TPTicketingPS.API.Controllers.V1;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public class UsersController(
    ICreateUser createUser,
    IGetUserById getUserById) : ControllerBase
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

    [HttpGet("{userId:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(
        [FromRoute] int userId,
        CancellationToken cancellationToken)
    {
        var user = await getUserById.ExecuteAsync(userId, cancellationToken);
        return Ok(user);
    }
}