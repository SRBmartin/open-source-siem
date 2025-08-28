namespace Siem.Platform.Logging.Domain.Entities;

public sealed class LogTag : Entity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Topic { get; private set; } = null!;

    public int Partitions { get; private set; }
    public long RetentionMs { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public bool IsDeleted { get; private set; }

    private LogTag() { }

    public static LogTag Create(
        string name,
        string topic,
        int partitions,
        long retentionMs,
        Guid? createdByUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is required");
        if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("topic is required");
        if (partitions <= 0) throw new ArgumentOutOfRangeException(nameof(partitions));
        if (retentionMs <= 0) throw new ArgumentOutOfRangeException(nameof(retentionMs));

        return new LogTag
        {
            Id = Guid.NewGuid(),
            Name = name.Trim().ToLowerInvariant(),
            Topic = topic.Trim(),
            Partitions = partitions,
            RetentionMs = retentionMs,
            CreatedByUserId = createdByUserId
        };
    }

    public void MarkDeleted() => IsDeleted = true;

    public void UpdateRetention(long retentionMs)
    {
        if (retentionMs <= 0) throw new ArgumentOutOfRangeException(nameof(retentionMs));
        RetentionMs = retentionMs;
    }

    public void UpdatePartitions(int partitions)
    {
        if (partitions <= 0) throw new ArgumentOutOfRangeException(nameof(partitions));
        Partitions = partitions;
    }
}

