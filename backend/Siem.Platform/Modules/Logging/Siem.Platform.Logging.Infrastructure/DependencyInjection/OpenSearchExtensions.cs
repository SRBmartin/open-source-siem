using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Client;

namespace Siem.Platform.Logging.Infrastructure.DependencyInjection;

public static class OpenSearchExtensions
{
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, Uri uri)
    {
        var settings = new ConnectionSettings(uri)
            .DefaultIndex("logs-*");

        var client = new OpenSearchClient(settings);
        services.AddSingleton<IOpenSearchClient>(client);

        return services;
    }
}
