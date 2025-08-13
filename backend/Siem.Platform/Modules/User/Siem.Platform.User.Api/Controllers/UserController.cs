using EBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.User.Api.Http;
using Siem.Platform.User.Api.Security;
using Siem.Platform.User.Application.DTOs.User;
using Siem.Platform.User.Application.Features.User.CreateUser;

namespace Siem.Platform.User.Api.Controllers;

[Route("api/[controller]")]
[RequireBearerToken]
[ApiController]
public class UserController (
    IMediator mediator
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto body, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(body.Email, body.FirstName, body.LastName);
        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }
}
