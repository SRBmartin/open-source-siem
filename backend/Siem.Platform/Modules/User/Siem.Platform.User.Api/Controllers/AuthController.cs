using EBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.User.Application.DTOs.Auth;
using Siem.Platform.User.Application.Features.Auth.VerifyEmail;

namespace Siem.Platform.User.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController (
    IMediator mediator  
) : ControllerBase
{
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request, CancellationToken cancellationToken)
    {
        var command = new VerifyEmailCommand(request.UserId, request.ActivationToken);
        var response = await mediator.Send(command, cancellationToken);

        return response.IsSuccess ? Ok() : BadRequest(response.Errors);
    }

}
