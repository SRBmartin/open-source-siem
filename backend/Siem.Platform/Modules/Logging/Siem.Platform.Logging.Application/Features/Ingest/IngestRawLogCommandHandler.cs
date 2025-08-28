using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Ingest;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Ingest;

public sealed class IngestRawLogCommandHandler(
    ILogTagRepository logTagRepository,
    ILogTagAccessRepository logTagAccessRepository,
    ICollectorIngestionService collectorIngestionService,
    ILogger<IngestRawLogCommandHandler> logger
) : IRequestHandler<IngestRawLogCommand, Result>
{
    public async Task<Result> Handle(IngestRawLogCommand cmd, CancellationToken cancellationToken)
    {
        var tag = await logTagRepository.GetByIdAsync(cmd.TagId, cancellationToken);
        if (tag is null)
            return Result.Failure(new Error("logging.tag.not_found", "Tag not found."));

        var acl = await logTagAccessRepository.GetAsync(cmd.TagId, cmd.UserId, cancellationToken);
        if (acl is null)
            return Result.Failure(new Error("logging.access.denied", "You do not have access to this tag."));

        var entry = new RawLogEntry(cmd.Timestamp, cmd.Message, cmd.Severity, cmd.Attributes);
        await collectorIngestionService.SendAsync(tag.Topic, entry, cancellationToken);

        return Result.Success();
    }
}