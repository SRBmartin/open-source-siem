using Iam.Platform.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Iam.Platform.Api.Security;

public static class AuthenticationExtensions
{

    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var kc = config.GetSection("Keycloak").Get<KeycloakSettings>()
                 ?? throw new InvalidOperationException("Keycloak configuration section is missing.");

        var authority = $"{kc.BaseUrl}/realms/{kc.Realm}";

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateAudience = false
                };

                options.IncludeErrorDetails = true;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(RequireClientAccessTokenAttribute.PolicyName, policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        });

        return services;
    }

}
