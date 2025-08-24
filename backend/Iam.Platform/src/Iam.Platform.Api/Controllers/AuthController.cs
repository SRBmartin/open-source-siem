using EBus.Abstractions;
using Iam.Platform.Application.DTOs.Auth;
using Iam.Platform.Application.Features.Auth.IntrospectToken;
using Iam.Platform.Application.Features.Auth.Login;
using Iam.Platform.Application.Features.Auth.Logout;
using Iam.Platform.Application.Models;
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

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Username, request.Password);
        
        var response = await mediator.Send(command, cancellationToken);

        return response.Success ? Ok(response) : Unauthorized(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(request.UserId);

        var response = await mediator.Send(command, cancellationToken);

        return response.Success ? Ok(response) : BadRequest(response.Message);
    }

}
