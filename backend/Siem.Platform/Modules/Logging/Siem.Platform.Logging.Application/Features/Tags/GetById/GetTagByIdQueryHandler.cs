using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Requests.Tags;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Tags.GetById;

public sealed class GetTagByIdQueryHandler(
    ILogTagRepository logTagRepository,
    ILogTagAccessRepository logTagAccessRepository,
    IIngestInfoService ingestInfoService,
    ILogger<GetTagByIdQueryHandler> logger
) : IRequestHandler<GetTagByIdQuery, Result<TagDetailDto>>
{
    public async Task<Result<TagDetailDto>> Handle(GetTagByIdQuery q, CancellationToken ct)
    {
        var tag = await logTagRepository.GetByIdAsync(q.TagId, ct);
        if (tag is null)
            return Result<TagDetailDto>.Failure(new Error("logging.tag.not_found", "Tag not found."));

        var myAcl = await logTagAccessRepository.GetAsync(q.TagId, q.UserId, ct);
        if (myAcl is null)
            return Result<TagDetailDto>.Failure(new Error("logging.access.denied", "You do not have access to this tag."));

        var all = await logTagAccessRepository.GetForTagAsync(q.TagId, ct);
        var accDtos = all
            .OrderBy(a => a.UserId)
            .Select(a => new TagAccessDto(a.UserId, a.Role.ToString()))
            .ToList()
            .AsReadOnly();

        var dto = new TagDetailDto(
            TagId: tag.Id,
            Name: tag.Name,
            Topic: tag.Topic,
            Partitions: tag.Partitions,
            RetentionMs: tag.RetentionMs,
            Ingest: ingestInfoService.Build(tag.Topic),
            Accesses: accDtos
        );

        return Result<TagDetailDto>.Success(dto);
    }
}