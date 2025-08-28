namespace Siem.Platform.Logging.Application.DTOs.Requests.Tags;

public sealed record CreateTagRequest(
    string Name,
    int? Partitions,
    int? RetentionDays
);
