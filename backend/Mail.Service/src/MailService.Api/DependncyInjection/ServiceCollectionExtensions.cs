using MailService.Application.DependencyInjection;
using MailService.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MailService.Api.DependncyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddInfrastructureServices(config);
        services.AddApplicationServices();

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Email service API", Version = "v1" });

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

        return services;
    }

}
