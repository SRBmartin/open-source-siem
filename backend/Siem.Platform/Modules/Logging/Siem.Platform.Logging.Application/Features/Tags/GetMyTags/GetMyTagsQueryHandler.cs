using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Requests.Tags;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Tags.GetMyTags;

public class GetMyTagsQueryHandler (
    ILogTagRepository logTagRepository,
    ILogTagAccessRepository logTagAccessRepository,
    IIngestInfoService ingestInfoService,
    ILogger<GetMyTagsQueryHandler> logger
) : IRequestHandler<GetMyTagsQuery, Result<IReadOnlyList<TagListItemDto>>>
{
    public async Task<Result<IReadOnlyList<TagListItemDto>>> Handle(GetMyTagsQuery q, CancellationToken ct)
    {
        // access rows for the user
        var myAccess = await logTagAccessRepository.GetForUserAsync(q.UserId, ct);
        if (myAccess.Count == 0)
            return Result<IReadOnlyList<TagListItemDto>>.Success(Array.Empty<TagListItemDto>());

        var tagIds = myAccess.Select(a => a.TagId).Distinct().ToArray();
        var myAccessByTag = myAccess.ToDictionary(a => a.TagId, a => a);

        var myTags = await logTagRepository.GetByIdsAsync(tagIds, ct);

        var list = myTags
            .OrderBy(t => t.Name)
            .Select(t =>
            {
                var acl = myAccessByTag[t.Id];
                var roleName = acl.Role.ToString();
                return new TagListItemDto(
                    TagId: t.Id,
                    Name: t.Name,
                    Topic: t.Topic,
                    MyRole: roleName,
                    Ingest: ingestInfoService.Build(t.Topic)
                );
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<TagListItemDto>>.Success(list);
    }

}
