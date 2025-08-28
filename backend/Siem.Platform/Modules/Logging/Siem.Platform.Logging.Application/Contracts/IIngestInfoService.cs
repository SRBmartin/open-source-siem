using Siem.Platform.Logging.Application.DTOs.Tags;

namespace Siem.Platform.Logging.Application.Contracts;

public interface IIngestInfoService
{
    IngestInstructionsDto Build(string topic);
}
