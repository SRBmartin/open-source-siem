using EBus.Abstractions;
using Siem.Platform.Logging.Application.DTOs.Requests.Tags;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Tags.GetMyTags;

public sealed record GetMyTagsQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<TagListItemDto>>>;