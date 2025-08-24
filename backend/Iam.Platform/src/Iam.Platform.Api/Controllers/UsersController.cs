using EBus.Abstractions;
using Iam.Platform.Api.Utilities;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Features.User.ActivateEmail;
using Iam.Platform.Application.Features.User.ChangePassword;
using Iam.Platform.Application.Features.User.CreateUser;
using Iam.Platform.Application.Features.User.DeleteUser;
using Iam.Platform.Application.Features.User.ExistsUser;
using Iam.Platform.Application.Features.User.ModifyRole;
using Microsoft.AspNetCore.Mvc;

namespace Iam.Platform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost]
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
    public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(Delete));
    }

    [HttpGet("exists")]
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
    public async Task<IActionResult> ModifyRole([FromRoute] string id, [FromBody] ModifyUserRoleRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ModifyRoleCommand(id, request.Role, request.Action);

        var response = await mediator.Send(command, cancellationToken);

        return response.ToActionResult(this, nameof(ModifyRole));
    }

}
