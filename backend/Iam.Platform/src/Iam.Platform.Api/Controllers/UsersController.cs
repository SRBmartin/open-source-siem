using EBus.Abstractions;
using Iam.Platform.Api.Utilities;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Features.User.CreateUser;
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
}
