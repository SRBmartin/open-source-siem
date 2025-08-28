namespace Siem.Platform.Logging.Infrastructure.Kafka.Configuration;

public class AuthOptions
{
    public string TokenUrl { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string Audience { get; set; } = default!;
}
