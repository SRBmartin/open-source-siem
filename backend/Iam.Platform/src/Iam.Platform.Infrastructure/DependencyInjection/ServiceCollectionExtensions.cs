using Iam.Platform.Application.Interfaces;
using Iam.Platform.Infrastructure.Configuration;
using Iam.Platform.Infrastructure.Identity;
using Iam.Platform.Infrastructure.Identity.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;

namespace Iam.Platform.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<KeycloakSettings>(config.GetSection("Keycloak"));

        services.AddTransient<KeycloakTokenHandler>();

        services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<IKeycloakUserService, KeycloakUserService>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<KeycloakTokenHandler>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient<ITokenValidationService, KeycloakTokenValidationService>((provider, client) =>
        {
            var opts = provider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }

}
