using EBus.Abstractions;
using Iam.Platform.Api.Utilities;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Features.User.ActivateEmail;
using Iam.Platform.Application.Features.User.CreateUser;
using Iam.Platform.Application.Features.User.DeleteUser;
using Iam.Platform.Application.Features.User.ExistsUser;
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

}
