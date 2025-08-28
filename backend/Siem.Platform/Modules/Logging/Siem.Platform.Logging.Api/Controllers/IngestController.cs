using EBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.Logging.Application.DTOs.Requests.Ingest;
using Siem.Platform.Logging.Application.Features.Ingest;
using Siem.Platform.Shared.Application.Abstractions.Security;

namespace Siem.Platform.Logging.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[RequireBearerToken]
public class IngestController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost("{tagId:guid}")]
    public async Task<IActionResult> Ingest(Guid tagId, [FromBody] IngestRequest req, CancellationToken ct)
    {
        var userIdStr = User.GetUserId();
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var cmd = new IngestRawLogCommand(
            TagId: tagId,
            UserId: userId,
            Timestamp: req.Timestamp ?? DateTimeOffset.UtcNow,
            Message: req.Message,
            Severity: req.Severity,
            Attributes: req.Attributes
        );

        var result = await mediator.Send(cmd, ct);
        return result.IsSuccess ? Accepted() : BadRequest(result.Errors);
    }
}
