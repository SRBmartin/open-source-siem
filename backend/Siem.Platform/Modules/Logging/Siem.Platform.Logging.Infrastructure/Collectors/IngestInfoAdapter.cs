using Microsoft.Extensions.Options;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Tags;
using Siem.Platform.Logging.Infrastructure.Kafka.Configuration;

namespace Siem.Platform.Logging.Infrastructure.Collectors;

class IngestInfoAdapter (
    IOptions<CollectorOptions> collectorOptions,
    IOptions<AuthOptions> authOptions
) : IIngestInfoService
{
    private readonly CollectorOptions _collectorOptions = collectorOptions.Value;
    private readonly AuthOptions _authOptions = authOptions.Value;

    public IngestInstructionsDto Build(string topic) =>
        new(
            _collectorOptions.PublicOtlpHttpUrl,
            _collectorOptions.HeaderName,
            topic,
            _authOptions.TokenUrl,
            _authOptions.ClientId,
            _authOptions.Audience
        );

}
