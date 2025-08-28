using Microsoft.Extensions.Options;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Ingest;
using Siem.Platform.Logging.Infrastructure.Kafka.Configuration;
using System.Net.Http.Json;

namespace Siem.Platform.Logging.Infrastructure.Collectors;

public sealed class OtlpCollectorClient(
    HttpClient http,
    IOptions<CollectorOptions> options
) : ICollectorIngestionService
{
    private readonly CollectorOptions _options = options.Value;

    public async Task SendAsync(string topic, RawLogEntry entry, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "");
        req.Headers.TryAddWithoutValidation(_options.HeaderName, topic);

        var nowNs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000L;
        var tsNs = entry.Timestamp.ToUnixTimeMilliseconds() * 1_000_000L;

        var attrs = (entry.Attributes ?? new Dictionary<string, string>())
            .Select(kv => new { key = kv.Key, value = new { stringValue = kv.Value } });

        var body = new
        {
            resourceLogs = new[] {
                new {
                    resource = new {
                        attributes = new object[] {
                            new { key = "service.name", value = new { stringValue = "siem-platform" } }
                        }
                    },
                    scopeLogs = new[] {
                        new {
                            scope = new { name = "siem.logging.api" },
                            logRecords = new[] {
                                new {
                                    timeUnixNano = tsNs.ToString(),
                                    observedTimeUnixNano = nowNs.ToString(),
                                    severityText = entry.Severity,
                                    body = new { stringValue = entry.Message },
                                    attributes = attrs
                                }
                            }
                        }
                    }
                }
            }
        };

        req.Content = JsonContent.Create(body);
        using var resp = await http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();
    }
}