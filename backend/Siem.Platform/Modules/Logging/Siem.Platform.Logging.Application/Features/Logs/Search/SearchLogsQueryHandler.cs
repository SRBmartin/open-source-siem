using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Logs;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Logs.Search;

public sealed class SearchLogsQueryHandler(
    ILogTagRepository logTagRepository,
    ILogSearchService logSearch,
    ILogger<SearchLogsQueryHandler> logger
) : IRequestHandler<SearchLogsQuery, Result<LogSearchResultDto>>
{
    public async Task<Result<LogSearchResultDto>> Handle(SearchLogsQuery q, CancellationToken ct)
    {
        var req = q.Request;
        string? topic = req.Topic;

        if (req.TagId is Guid tagId)
        {
            logger.LogInformation("Searching for tag with ID: {TagId}", tagId);

            var tag = await logTagRepository.GetByIdAsync(tagId, ct);
            if (tag is null)
            {
                logger.LogWarning("Tag not found for ID: {TagId}", tagId);
                return Result<LogSearchResultDto>.Failure(
                    new Error("logging.tag.not_found", "Tag not found."));
            }

            logger.LogInformation("Found tag with topic: '{Topic}' for TagId: {TagId}", tag.Topic, tagId);
            topic = tag.Topic;
        }

        logger.LogInformation("Executing search with topic: '{Topic}', original request topic: '{OriginalTopic}'",
            topic, req.Topic);

        var normalized = req with { Topic = topic };

        logger.LogDebug("Normalized search request: {@Request}", normalized);

        try
        {
            var result = await logSearch.SearchAsync(normalized, ct);
            logger.LogInformation("Search completed successfully. Found {HitCount} hits", result.Hits.Count);
            return Result<LogSearchResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Search failed for topic: '{Topic}'", topic);
            throw;
        }
    }
}