namespace Siem.Platform.Logging.Application.Contracts;

public interface ITopicProvisionerService
{
    Task EnsureTopicsAsync(string tag, int? partitions, long? retentionMs, CancellationToken ct);

    string BuildTopic(string tag);
}
