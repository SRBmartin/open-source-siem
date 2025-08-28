namespace Siem.Platform.Logging.Application.Contracts;

public interface ILoggingDefaultsService
{
    int DefaultPartitions { get; }
    long DefaultRetentionMs { get; }
}
