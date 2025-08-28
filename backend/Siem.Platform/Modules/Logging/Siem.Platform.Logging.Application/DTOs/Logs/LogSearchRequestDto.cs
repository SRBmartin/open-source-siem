namespace Siem.Platform.Logging.Application.DTOs.Logs;

public sealed record LogSearchRequestDto(
    Guid? TagId,
    string? Topic,
    string? Text,         //query string (Lucene syntax)
    string? Severity,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int Size = 50,
    string? Cursor = null
);