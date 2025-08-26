using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Siem.Platform.Shared.Application.Abstractions.Services;
using Siem.Platform.Shared.Infrastructure.Configuration;
using Siem.Platform.Shared.Infrastructure.Http;
using Siem.Platform.Shared.Infrastructure.Services;
using Siem.Platform.Shared.Infrastructure.Services.Security;

namespace Siem.Platform.Shared.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        #region Configuration

        services.Configure<KeycloakSettings>(config.GetSection("Keycloak"));
        services.Configure<IamPlatformSettings>(config.GetSection("IamPlatformSettings"));
        services.Configure<ServicesConfiguration>(config.GetSection("Services"));

        #endregion


        #region Http

        services.AddTransient<ClientAccessTokenHandler>();

        services.AddResilientHttpClient<IKeycloakTokenService, KeycloakTokenService>(
            "token",
            baseAddressFactory: sp => new Uri(sp.GetRequiredService<IOptions<KeycloakSettings>>().Value.BaseUrl),
            timeout: TimeSpan.FromSeconds(30)
        );

        services.AddResilientHttpClient<IAuthVerificationService, AuthVerificationService>(
            "auth-verification",
            baseAddressFactory: sp => new Uri(sp.GetRequiredService<IOptions<IamPlatformSettings>>().Value.BaseUrl),
            timeout: TimeSpan.FromSeconds(30)
        );

        services.AddResilientHttpClient<IMailGateway, MailGateway>(
            "mail-service",
            baseAddressFactory: sp => new Uri(sp.GetRequiredService<IOptions<ServicesConfiguration>>().Value.MailService),
            timeout: TimeSpan.FromSeconds(30)
        )
        .AddHttpMessageHandler<ClientAccessTokenHandler>();

        #endregion

        #region Other

        services.AddMemoryCache();

        #endregion

        return services;
    }

}
