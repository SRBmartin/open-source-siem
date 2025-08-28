namespace Siem.Platform.Logging.Infrastructure.Kafka.Configuration;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = string.Empty;
    public int ReplicationFactor { get; set; } = 3;
    public int MinInSyncReplicas { get; set; } = 2;
    public int DefaultPartitions { get; set; } = 6;
    public long DefaultRetentionMs { get; set; } = 604800000;
    public string TopicPrefix { get; set; } = "logs";
}
