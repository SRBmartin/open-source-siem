using Keycloak.Seeder.Application.Contracts;
using Keycloak.Seeder.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Seeder.Application.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISeederWorker, SeederWorker>();

        return services;
    }

}
