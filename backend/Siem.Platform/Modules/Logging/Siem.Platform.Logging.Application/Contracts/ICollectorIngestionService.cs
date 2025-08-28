using Siem.Platform.Logging.Application.DTOs.Ingest;

namespace Siem.Platform.Logging.Application.Contracts;

public interface ICollectorIngestionService
{
    Task SendAsync(string topic, RawLogEntry entry, CancellationToken cancellationToken);
}
