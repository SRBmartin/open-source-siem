using OpenSearch.Client;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Logs;
using Microsoft.Extensions.Options;
using Siem.Platform.Logging.Infrastructure.Kafka.Configuration;
using System.Text.Json;

namespace Siem.Platform.Logging.Infrastructure.Services;

class OpenSearchLogSearchService (
    IOpenSearchClient client
) : ILogSearchService
{
    public async Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto req, CancellationToken ct)
    {
        var filters = new List<QueryContainer>();
        if (!string.IsNullOrWhiteSpace(req.Topic))
        {
            var suffix = req.Topic!.EndsWith(".v1", StringComparison.OrdinalIgnoreCase)
                ? req.Topic!
                : $"{req.Topic}.v1";

            filters.Add(new BoolQuery
            {
                Should = new QueryContainer[]
                {
                    new TermQuery     { Field = "siem_topic.keyword", Value = req.Topic },
                    new TermQuery     { Field = "siem_topic",         Value = req.Topic },

                    new TermQuery     { Field = "topic.keyword",      Value = suffix },
                    new TermQuery     { Field = "topic",              Value = suffix },

                    new WildcardQuery { Field = "topic.keyword",      Value = $"*.{suffix}" },
                    new WildcardQuery { Field = "topic",              Value = $"*.{suffix}" }
                },
                MinimumShouldMatch = 1
            });
        }

        if (!string.IsNullOrWhiteSpace(req.Severity))
        {
            filters.Add(new BoolQuery
            {
                Should = new QueryContainer[]
                {
            new TermQuery { Field = "severity_text.keyword", Value = req.Severity },
            new TermQuery { Field = "severity.keyword",      Value = req.Severity },
            new TermQuery { Field = "severity",              Value = req.Severity },
            
            new TermQuery { Field = "payload.resourceLogs.scopeLogs.logRecords.severityText.keyword", Value = req.Severity },
            new TermQuery { Field = "payload.resourceLogs.scopeLogs.logRecords.severityText",         Value = req.Severity }
                },
                MinimumShouldMatch = 1
            });
        }

        if (req.From is not null || req.To is not null)
        {
            var range = new DateRangeQuery { Field = "@timestamp" };

            if (req.From.HasValue)
                range.GreaterThanOrEqualTo = req.From.Value.UtcDateTime;

            if (req.To.HasValue)
                range.LessThanOrEqualTo = req.To.Value.UtcDateTime;

            filters.Add(range);
        }

        QueryContainer? must = null;
        if (!string.IsNullOrWhiteSpace(req.Text))
        {
            must = new QueryStringQuery
            {
                Query = req.Text,
                DefaultOperator = Operator.And,
                DefaultField = "*",
                Lenient = true,
                AnalyzeWildcard = true
            };
        }

        object[]? searchAfter = null;
        if (!string.IsNullOrEmpty(req.Cursor))
        {
            var bytes = Convert.FromBase64String(req.Cursor);
            searchAfter = JsonSerializer.Deserialize<object[]>(bytes);
        }

        var sreq = new SearchRequest("logs-*")
        {
            Size = req.Size,
            Sort = new List<ISort>
            {
                new FieldSort { Field = "@timestamp", Order = SortOrder.Descending },
                new FieldSort { Field = "_id",        Order = SortOrder.Descending }
            },
            Query = new BoolQuery
            {
                Filter = filters,
                Must = must is null ? null : new List<QueryContainer> { must }
            },
            SearchAfter = searchAfter
        };

        var resp = await client.SearchAsync<dynamic>(sreq, ct);
        if (!resp.IsValid)
            throw new InvalidOperationException(resp.OriginalException?.Message ?? resp.ServerError?.ToString());

        var hits = resp.Hits.Select(h =>
        {
            DateTimeOffset ts = default;
            if (h.Source is IDictionary<string, object> src && src.TryGetValue("@timestamp", out var tsv))
            {
                if (tsv is string s && DateTimeOffset.TryParse(s, out var parsed)) ts = parsed;
            }

            string? msg = TryGet(h.Source, "message") as string ?? TryGet(h.Source, "log") as string;
            string? sevText = TryGet(h.Source, "severity_text") as string;
            int? sevNum = TryGet(h.Source, "severity_number") as int?;
            string? siemTopic = TryGet(h.Source, "siem_topic") as string
                                ?? TryGet(h.Source, "siem_topic.keyword") as string;

            return new LogDocDto(
                Id: h.Id,
                Timestamp: ts,
                Message: msg,
                SeverityText: sevText,
                SeverityNumber: sevNum,
                SiemTopic: siemTopic,
                Source: h.Source as IReadOnlyDictionary<string, object>
            );
        }).ToList();

        string? nextCursor = null;
        var last = resp.Hits.LastOrDefault();
        if (last?.Sorts is not null && last.Sorts.Count > 0)
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(last.Sorts);
            nextCursor = Convert.ToBase64String(payload);
        }

        return new LogSearchResultDto(hits, nextCursor);

        static object? TryGet(object src, string key)
        {
            if (src is IDictionary<string, object> d && d.TryGetValue(key, out var v)) return v;
            return null;
        }
    }

}
