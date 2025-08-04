using Keycloak.Seeder.Application.Contracts;
using Keycloak.Seeder.Infrastructure.Configuration;
using Keycloak.Seeder.Infrastructure.Http;
using Keycloak.Seeder.Infrastructure.Http.Seeders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;

namespace Keycloak.Seeder.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<KeycloakOptions>(config.GetSection("Keycloak"));

        services.AddHttpClient<IKeycloakAdminAuthClient, KeycloakAdminAuthClient>();

        services.AddHttpClient<IKeycloakClientSeeder, KeycloakClientSeeder>(client =>
        {
            client.BaseAddress = new Uri(config["Keycloak:BaseUrl"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3)));

        services.AddHttpClient<IKeycloakRolesSeeder, KeycloakRolesSeeder>(client =>
        {
            client.BaseAddress = new Uri(config["Keycloak:BaseUrl"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(3)));

        return services;
    }
}
