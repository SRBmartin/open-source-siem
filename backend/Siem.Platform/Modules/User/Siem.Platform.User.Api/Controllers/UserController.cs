using EBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.User.Api.Http;
using Siem.Platform.User.Api.Security;
using Siem.Platform.User.Application.DTOs.User;
using Siem.Platform.User.Application.Features.User.ChangePassword;
using Siem.Platform.User.Application.Features.User.CreateUser;
using Siem.Platform.User.Application.Features.User.GetAllUsers;
using Siem.Platform.User.Application.Features.User.ModifyRole;

namespace Siem.Platform.User.Api.Controllers;

[Route("api/[controller]")]
[RequireBearerToken]
[ApiController]
public class UserController (
    IMediator mediator
) : ControllerBase
{
    [HttpPost]
    [RequireRealmRole(UserRoles.Administrator)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto body, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(body.Email, body.FirstName, body.LastName);
        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);

        var response = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(response);
    }

    [HttpPost("roles")]
    [RequireRealmRole(UserRoles.Administrator)]
    public async Task<IActionResult> ModifyRole([FromBody] ModifyUserRoleRequestDto request, CancellationToken cancellationToken)
    {
        var initiatorUserId = User.GetUserId();
        if (string.IsNullOrWhiteSpace(initiatorUserId))
        {
            return Unauthorized();
        }

        var command = new ModifyRoleCommand(
            initiatorUserId,
            request.TargetUserId,
            request.Role,
            request.Action
        );

        var response = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(response);
    }

    [HttpGet]
    [RequireRealmRole(UserRoles.Administrator)]
    public async Task<IActionResult> GetUsers([FromQuery] int? first, [FromQuery] int? max, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery(first, max, search);

        var response = await mediator.Send(query, cancellationToken);

        return this.ToActionResult(response);
    }

}
