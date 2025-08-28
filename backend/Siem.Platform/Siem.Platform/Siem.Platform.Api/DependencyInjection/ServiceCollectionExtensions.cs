using Siem.Platform.User.Api.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using EBus.Registration;
using Siem.Platform.Shared.Infrastructure.DependencyInjection;
using Siem.Platform.Logging.Api.DependencyInjection;

namespace Siem.Platform.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformApiServices(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        services.AddLogging(l =>
        {
            l.ClearProviders();
            l.AddSerilog(dispose: true);
        });

        services.AddEBus();

        services.AddSharedInfrastructureServices(config);

        services.AddUserApiServices(config);
        services.AddLoggingApiServices(config);

        var mvc = services.AddControllers();
        mvc.AddApplicationPart(typeof(ServiceCollectionExtensions).Assembly);
        mvc.AddLoggingModuleControllers();
        mvc.AddUserModuleControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Siem Platform API",
                Version = "v1"
            });

            const string schemeName = "Bearer";

            c.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = schemeName
                        }
                    },
                    Array.Empty<string>()
                }
            });

        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                //TODO: When added in configuration, uncomment the following lines
                //options.Authority = config["IdentityServer:Authority"];
                //options.RequireHttpsMetadata = false;
            });

        services.AddAuthorization(options =>
        {
            //TODO: When added policies, register here...
        });

        return services;
    }

}
