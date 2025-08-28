using EBus.Abstractions;
using Siem.Platform.Logging.Domain.Entities;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Tags.GrantAccess;

public sealed class GrantLogTagAccessCommandHandler (
    ILogTagRepository logTagRepository,
    ILogTagAccessRepository logTagAccessRepository
) : IRequestHandler<GrantLogTagAccessCommand, Result>
{
    public async Task<Result> Handle(GrantLogTagAccessCommand cmd, CancellationToken ct)
    {
        var tag = await logTagRepository.GetByIdAsync(cmd.TagId, ct);
        if (tag is null)
            return Result.Failure(new Error("logging.tag.not_found", "Tag not found."));

        var existing = await logTagAccessRepository.GetAsync(cmd.TagId, cmd.UserId, ct);
        if (existing is null)
        {
            await logTagAccessRepository.AddAsync(LogTagAccess.Grant(cmd.TagId, cmd.UserId, cmd.Role), ct);
            await logTagAccessRepository.SaveChangesAsync(ct);
        }
        else if (existing.Role != cmd.Role)
        {
            existing.Promote(cmd.Role);
            await logTagAccessRepository.SaveChangesAsync(ct);
        }

        return Result.Success();
    }

}
