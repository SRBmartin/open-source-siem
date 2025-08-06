using EBus.Abstractions;
using Iam.Platform.Application.Features.Auth.IntrospectToken;
using Microsoft.AspNetCore.Mvc;

namespace Iam.Platform.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController (
    IMediator mediator
) : ControllerBase
{
    [HttpPost("verify")]
    public async Task<IActionResult> ValidateToken(CancellationToken cancellationToken)
    {
        var header = Request.Headers["Authorization"].FirstOrDefault();

        var active = await mediator.Send(new IntrospectTokenCommand(header), cancellationToken);

        return active ? Ok() : Unauthorized(null!);
    }

}
