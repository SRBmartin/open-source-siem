using EBus.Abstractions;
using Iam.Platform.Api.Security;
using Iam.Platform.Api.Utilities;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Features.User.ActivateEmail;
using Iam.Platform.Application.Features.User.ChangePassword;
using Iam.Platform.Application.Features.User.CreateUser;
using Iam.Platform.Application.Features.User.DeleteUser;
using Iam.Platform.Application.Features.User.ExistsUser;
using Iam.Platform.Application.Features.User.GetUsers;
using Iam.Platform.Application.Features.User.ModifyRole;
using Iam.Platform.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Iam.Platform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost]
    [RequireClientAccessToken]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateUserCommand(dto), cancellationToken);

        return response.ToActionResult(this, nameof(Create));
    }

    [HttpPost("{id}/verify-email")]
    public async Task<IActionResult> VerifyEmail([FromRoute] string id, CancellationToken cancellationToken)
    {
        var command = new ActivateEmailCommand(id);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(VerifyEmail));
    }

    [HttpDelete("{id}")]
    [RequireClientAccessToken]
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(Delete));
    }

    [HttpGet("exists")]
    [RequireClientAccessToken]
    public async Task<IActionResult> Exists([FromQuery] string email, CancellationToken cancellationToken)
    {
        var command = new ExistsUserCommand(email);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(Exists));
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(ChangePassword));
    }

    [HttpPost("{id}/roles")]
    [RequireClientAccessToken]
    public async Task<IActionResult> ModifyRole([FromRoute] string id, [FromBody] ModifyUserRoleRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ModifyRoleCommand(id, request.Role, request.Action);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(ModifyRole));
    }

    [HttpGet]
    [RequireClientAccessToken]
    [ProducesResponseType(typeof(ApiResponse<List<UserListItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int first = 0,
        [FromQuery] int max = 50,
        [FromQuery] bool includeRoles = false,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetUsersQuery(
            First: first,
            Max: max,
            IncludeRoles: includeRoles,
            Search: search
        );

        var response = await mediator.Send(query, cancellationToken);

        return response.ToActionResult(this, nameof(GetUsers));
    }

}
