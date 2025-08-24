using EBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.User.Application.DTOs.Auth;
using Siem.Platform.User.Application.Features.Auth.LoginCommand;
using Siem.Platform.User.Application.Features.Auth.LogoutCommand;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Username, request.Password);

        var response = await mediator.Send(command, cancellationToken);

        return response.IsSuccess ? Ok(new LoginResponseDto(response.Value!)) : Unauthorized(response.Errors);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(request.UserId);

        var response = await mediator.Send(command, cancellationToken);

        return response.IsSuccess ? Ok() : BadRequest(response.Errors);
    }

}
