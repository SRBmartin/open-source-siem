namespace Siem.Platform.Logging.Application.DTOs.Logs;

public sealed record LogSearchResultDto(
    IReadOnlyList<LogDocDto> Hits,
    string? NextCursor
);