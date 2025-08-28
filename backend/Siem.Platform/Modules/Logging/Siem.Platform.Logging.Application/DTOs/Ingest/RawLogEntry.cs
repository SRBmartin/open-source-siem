namespace Siem.Platform.Logging.Application.DTOs.Ingest;
public sealed record RawLogEntry(
    DateTimeOffset Timestamp,
    string Message,
    string Severity, // "INFO", "WARN", "ERROR"
    IReadOnlyDictionary<string, string>? Attributes
);