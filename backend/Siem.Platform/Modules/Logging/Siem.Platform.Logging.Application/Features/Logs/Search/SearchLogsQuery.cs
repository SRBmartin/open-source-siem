using EBus.Abstractions;
using Siem.Platform.Logging.Application.DTOs.Logs;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Logs.Search;

public sealed record SearchLogsQuery(LogSearchRequestDto Request, Guid? RequestedBy)
    : IRequest<Result<LogSearchResultDto>>;
