using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siem.Platform.User.Domain.Repositories;
using Siem.Platform.User.Infrastructure.Persistence.Contexts;
using Siem.Platform.User.Infrastructure.Persistence.Repositories;
using Siem.Platform.User.Application.Contracts;
using Microsoft.Extensions.Options;
using Siem.Platform.User.Infrastructure.Services;
using Siem.Platform.Shared.Infrastructure.Configuration;

namespace Siem.Platform.User.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        #region Persistence

        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(
                config.GetConnectionString("PlatformDatabase"),
                sql => sql.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName)
            )
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IActivationTokenRepository, ActivationTokenRepository>();

        #endregion

        #region Http

        services.AddResilientHttpClient<IIdentityService, IdentityService>(
            pipelineName: "iam",
            baseAddressFactory: sp => new Uri(sp.GetRequiredService<IOptions<ServicesConfiguration>>().Value.IamPlatform),
            timeout: TimeSpan.FromSeconds(30)
        );

        #endregion

        #region Utils

        services.AddSingleton<IRandomGenerator, RandomGenerator>();

        #endregion

        return services;
    }

}
