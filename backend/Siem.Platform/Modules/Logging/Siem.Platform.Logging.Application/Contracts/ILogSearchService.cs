using Siem.Platform.Logging.Application.DTOs.Logs;

namespace Siem.Platform.Logging.Application.Contracts;

public interface ILogSearchService
{
    Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request, CancellationToken ct);
}