using Siem.Platform.Logging.Infrastructure.DependencyInjection;

namespace Siem.Platform.Logging.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddInfrastructureServices(config);

        return services;
    }

    public static IMvcBuilder AddLoggingModuleControllers(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(ServiceCollectionExtensions).Assembly);
        builder.AddControllersAsServices();
        return builder;
    }

}
