namespace Siem.Platform.Logging.Application.DTOs.Requests.Tags;

public sealed record TagListItemDto(
    Guid TagId,
    string Name,
    string Topic,
    string MyRole,
    object Ingest
);