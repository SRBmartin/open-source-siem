using EBus.Registration;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Services;
using Microsoft.Extensions.DependencyInjection; 

namespace Iam.Platform.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddEBus();

        services.AddScoped<IUserValidationService, UserValidationService>();

        return services;
    }

}
