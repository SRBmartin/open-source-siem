using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Ingest;

public sealed record IngestRawLogCommand(
    Guid TagId,
    Guid UserId,
    DateTimeOffset Timestamp,
    string Message,
    string Severity,
    IReadOnlyDictionary<string, string>? Attributes
) : IRequest<Result>;
