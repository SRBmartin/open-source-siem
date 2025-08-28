namespace Siem.Platform.Logging.Application.DTOs.Tags;

public sealed record CreateLogTagResultDto(
    Guid TagId,
    string Name,
    string Topic,
    IngestInstructionsDto Ingest);
