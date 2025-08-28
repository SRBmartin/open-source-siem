namespace Siem.Platform.Logging.Application.DTOs.Requests.Tags;

public sealed record TagDetailDto(
    Guid TagId,
    string Name,
    string Topic,
    int Partitions,
    long RetentionMs,
    object Ingest,
    IReadOnlyList<TagAccessDto> Accesses
);