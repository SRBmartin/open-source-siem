using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Siem.Platform.Logging.Infrastructure.DependencyInjection;

public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddResilientHttpClient<TClient, TImplementation>(
        this IServiceCollection services,
        string pipelineName,
        Func<IServiceProvider, Uri> baseAddressFactory,
        TimeSpan? timeout = null
    ) 
        where TClient : class
        where TImplementation : class, TClient
    {
        var httpClientBuilder = services.AddHttpClient<TClient, TImplementation>((sp, client) =>
        {
            client.BaseAddress = baseAddressFactory(sp);
            if (timeout is not null) client.Timeout = timeout.Value;
        });

        httpClientBuilder.AddResilienceHandler(pipelineName, builder =>
        {
            AddStandardHttpResilience(builder);
        });

        return httpClientBuilder;
    }

    private static void AddStandardHttpResilience(ResiliencePipelineBuilder<HttpResponseMessage> builder)
    {
        builder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        });
    }

}
