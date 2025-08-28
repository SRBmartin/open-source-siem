using Confluent.Kafka.Admin;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Infrastructure.Kafka.Configuration;

namespace Siem.Platform.Logging.Infrastructure.Kafka.Services;

class KafkaTopicProvisionerAdapter (
    IOptions<KafkaOptions> options    
) : ITopicProvisionerService, ILoggingDefaultsService
{
    private readonly KafkaOptions _options = options.Value;

    public string BuildTopic(string tag) => $"{_options.TopicPrefix}.{tag}.v1";
    private string BuildDlq(string tag) => $"{_options.TopicPrefix}.{tag}.dlq.v1";

    public int DefaultPartitions => _options.DefaultPartitions;
    public long DefaultRetentionMs => _options.DefaultRetentionMs;

    public async Task EnsureTopicsAsync(string tag, int? partitions, long? retentionMs, CancellationToken cancellationToken)
    {
        var cfg = new AdminClientConfig { BootstrapServers = _options.BootstrapServers };
        using var admin = new AdminClientBuilder(cfg).Build();

        var p = partitions ?? _options.DefaultPartitions;
        var r = retentionMs ?? _options.DefaultRetentionMs;
        var rf = (short)_options.ReplicationFactor;

        var specs = new[]
        {
            new TopicSpecification {
                Name = BuildTopic(tag), NumPartitions = p, ReplicationFactor = rf,
                Configs = new() {
                  ["min.insync.replicas"] = _options.MinInSyncReplicas.ToString(),
                  ["unclean.leader.election.enable"] = "false",
                  ["retention.ms"] = r.ToString(),
                  ["compression.type"] = "gzip"
                }},
            new TopicSpecification {
                Name = BuildDlq(tag), NumPartitions = Math.Max(1, p/2), ReplicationFactor = rf,
                Configs = new() {
                  ["min.insync.replicas"] = _options.MinInSyncReplicas.ToString(),
                  ["unclean.leader.election.enable"] = "false",
                  ["retention.ms"] = (30L*24*60*60*1000).ToString()
                }}
        };

        try { await admin.CreateTopicsAsync(specs); }
        catch (CreateTopicsException e)
        {
            if (e.Results.Any(r => r.Error.Code != ErrorCode.TopicAlreadyExists)) throw;
        }
    }

}
