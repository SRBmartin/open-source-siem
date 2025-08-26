using EBus.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Siem.Platform.User.Application.Utils;

namespace Siem.Platform.User.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly, includeInternalTypes: true);

        services.AddTransient(typeof(IPipelineBehaviour<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
