namespace Siem.Platform.Logging.Infrastructure.Kafka.Configuration;

public class CollectorOptions
{
    public string PublicOtlpHttpUrl { get; set; } = default!;
    public string? InternalOtlpHttpUrl { get; set; } //docker net
    public string HeaderName { get; set; } = default!;
}
