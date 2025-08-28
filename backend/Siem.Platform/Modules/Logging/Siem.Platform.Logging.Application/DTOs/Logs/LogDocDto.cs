namespace Siem.Platform.Logging.Application.DTOs.Logs;

public sealed record LogDocDto(
    string Id,
    DateTimeOffset Timestamp,
    string? Message,
    string? SeverityText,
    int? SeverityNumber,
    string? SiemTopic,
    IReadOnlyDictionary<string, object>? Source
);