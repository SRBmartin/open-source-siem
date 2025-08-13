using Siem.Platform.User.Infrastructure.DependencyInjection;

namespace Siem.Platform.User.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddInfrastructureServices(config);

        return services;
    }

    public static IMvcBuilder AddUserModuleControllers(this IMvcBuilder builder)
    {
        builder.AddApplicationPart(typeof(ServiceCollectionExtensions).Assembly);
        builder.AddControllersAsServices();

        return builder;
    }

}
