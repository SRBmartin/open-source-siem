using EBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.Logging.Application.DTOs.Logs;
using Siem.Platform.Logging.Application.Features.Logs.Search;
using Siem.Platform.Shared.Application.Abstractions.Security;

namespace Siem.Platform.Logging.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[RequireBearerToken]
public class LogsController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] LogSearchRequestDto request, CancellationToken ct)
    {
        Guid? requestedBy = null;
        var idStr = User.GetUserId();
        if (Guid.TryParse(idStr, out var uid)) requestedBy = uid;

        var result = await mediator.Send(new SearchLogsQuery(request, requestedBy), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
