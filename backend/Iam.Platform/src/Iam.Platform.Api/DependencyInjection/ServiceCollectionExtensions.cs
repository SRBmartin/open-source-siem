using Iam.Platform.Application.DependencyInjection;
using Iam.Platform.Infrastructure.DependencyInjection;
using Serilog;
using Microsoft.OpenApi.Models;
using Iam.Platform.Api.Security;

namespace Iam.Platform.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, dispose: true);
        });

        services.AddAuthorization();

        services.AddControllers();
        
        services.AddApplicationServices();
        services.AddInfrastructureServices(config);

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Iam.Platform API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter your JWT token as: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name   = "Authorization",
                        In     = ParameterLocation.Header,
                    },
                    Array.Empty<string>()
                }
            });

        });

        services.AddKeycloakAuthentication(config);

        return services;
    }

}
