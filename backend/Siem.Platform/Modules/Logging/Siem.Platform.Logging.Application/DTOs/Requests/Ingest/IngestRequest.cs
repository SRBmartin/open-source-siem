namespace Siem.Platform.Logging.Application.DTOs.Requests.Ingest;

public sealed record IngestRequest(
        string Message,
        string Severity,
        Dictionary<string, string>? Attributes,
        DateTimeOffset? Timestamp
);