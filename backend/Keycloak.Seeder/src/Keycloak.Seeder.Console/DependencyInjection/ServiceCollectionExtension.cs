using Keycloak.Seeder.Application.DependencyInjection;
using Keycloak.Seeder.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Keycloak.Seeder.Console.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConsoleServices(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, dispose: true);
        });

        services.AddInfrastructureServices(config);
        services.AddApplicationServices();

        return services;
    }
}
