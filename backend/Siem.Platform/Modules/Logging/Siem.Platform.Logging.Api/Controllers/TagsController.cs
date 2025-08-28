using EBus.Abstractions;
using EBus.Implementation;
using Microsoft.AspNetCore.Mvc;
using Siem.Platform.Logging.Application.DTOs.Requests.Tags;
using Siem.Platform.Logging.Application.Features.Tags.Create;
using Siem.Platform.Logging.Application.Features.Tags.GetById;
using Siem.Platform.Logging.Application.Features.Tags.GetMyTags;
using Siem.Platform.Logging.Application.Features.Tags.GrantAccess;
using Siem.Platform.Shared.Application.Abstractions.Security;

namespace Siem.Platform.Logging.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[RequireBearerToken]
public class TagsController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        Guid? createdBy = null;
        var idStr = User.GetUserId();
        if (Guid.TryParse(idStr, out var uid)) createdBy = uid;

        var cmd = new CreateLogTagCommand(request.Name, request.Partitions, request.RetentionDays, CreatedBy: createdBy);
        var result = await mediator.Send(cmd, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost("{tagId:guid}/access")]
    public async Task<IActionResult> Grant(Guid tagId, [FromBody] GrantAccessRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GrantLogTagAccessCommand(tagId, request.UserId, request.Role), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTags(CancellationToken cancellationToken)
    {
        var idStr = User.GetUserId();
        if (!Guid.TryParse(idStr, out var userId)) return Unauthorized();

        var query = new GetMyTagsQuery(userId);
        var result = await mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("{tagId:guid}")]
    public async Task<IActionResult> GetById(Guid tagId, CancellationToken cancellationToken)
    {
        var idStr = User.GetUserId();
        if (!Guid.TryParse(idStr, out var userId)) return Unauthorized();

        var query = new GetTagByIdQuery(tagId, userId);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            var code = result.Errors.FirstOrDefault()?.Code;
            if (code == "logging.tag.not_found") return NotFound(result.Errors);
            if (code == "logging.access.denied") return Forbid();
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

}
