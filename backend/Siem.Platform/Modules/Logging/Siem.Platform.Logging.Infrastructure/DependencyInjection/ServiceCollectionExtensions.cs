using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Logging.Infrastructure.Collectors;
using Siem.Platform.Logging.Infrastructure.Kafka.Configuration;
using Siem.Platform.Logging.Infrastructure.Kafka.Services;
using Siem.Platform.Logging.Infrastructure.Persistence.Contexts;
using Siem.Platform.Logging.Infrastructure.Persistence.Repositories;
using Siem.Platform.Shared.Infrastructure.Http;

namespace Siem.Platform.Logging.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<KafkaOptions>(config.GetSection("Kafka"));
        services.Configure<CollectorOptions>(config.GetSection("Collector"));
        services.Configure<AuthOptions>(config.GetSection("Auth"));

        services.AddDbContext<LoggingDbContext>(options =>
            options.UseNpgsql(
                config.GetConnectionString("PlatformDatabase"),
                sql => sql.MigrationsAssembly(typeof(LoggingDbContext).Assembly.FullName)
            )
        );

        services.AddScoped<ILogTagRepository, LogTagRepository>();
        services.AddScoped<ILogTagAccessRepository, LogTagAccessRepository>();

        services.AddSingleton<ITopicProvisionerService, KafkaTopicProvisionerAdapter>();
        services.AddSingleton<ILoggingDefaultsService, KafkaTopicProvisionerAdapter>();
        services.AddSingleton<IIngestInfoService, IngestInfoAdapter>();

        services.AddResilientHttpClient<ICollectorIngestionService, OtlpCollectorClient>(
            "otel-collector",
            baseAddressFactory: sp =>
            {
                var opt = sp.GetRequiredService<IOptions<CollectorOptions>>().Value;
                var url = opt.InternalOtlpHttpUrl ?? opt.PublicOtlpHttpUrl;
                return new Uri(url);
            }
        )
        .AddHttpMessageHandler<ClientAccessTokenHandler>();

        return services;
    }

}
