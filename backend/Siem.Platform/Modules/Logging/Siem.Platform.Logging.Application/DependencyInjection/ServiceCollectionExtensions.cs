using EBus.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Siem.Platform.Logging.Application.Utils;

namespace Siem.Platform.Logging.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly, includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehaviour<,>), typeof(ValidationBehavior<,>));

        return services;
    }

}
