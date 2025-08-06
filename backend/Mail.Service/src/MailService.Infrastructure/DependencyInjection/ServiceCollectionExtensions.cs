using MailService.Application.Interfaces;
using MailService.Application.Interfaces;
using MailService.Infrastructure.Configuration;
using MailService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using RazorLight;
using RazorLight.Razor;
using System.Net.Mail;

namespace MailService.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        services.Configure<UrisSettings>(config.GetSection("UrisSettings"));
        services.Configure<IamPlatformSettings>(config.GetSection("IamPlatformSettings"));

        services.AddSingleton<IRazorLightEngine>(sp =>
        {
            var asm = typeof(SmtpEmailService).Assembly;
            var project = new EmbeddedRazorProject(
                    asm,
                    rootNamespace: "MailService.Infrastructure.Resources"
                );

            return new RazorLightEngineBuilder()
                .UseProject(project)
                .UseMemoryCachingProvider()
                .Build();
        });

        var registry = services.AddPolicyRegistry();
        registry.Add<IAsyncPolicy>("MailRetryPolicy",
            Policy.Handle<SmtpException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, delay) => { /* Retry logging sometime maybe */ })
        );

        services.AddSingleton(sp => sp.GetRequiredService<IReadOnlyPolicyRegistry<string>>()
            .Get<IAsyncPolicy>("MailRetryPolicy"));

        services.AddSingleton<IEmailService, SmtpEmailService>();

        services.AddScoped<IEmailVerificationService, EmailVerificationService>();

        services.AddHttpClient<IAuthVerificationService, AuthVerificationService>((provider, client) =>
        {
            var opts = provider.GetRequiredService<IOptions<IamPlatformSettings>>().Value;
            client.BaseAddress = new Uri(opts.BaseUrl);
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    }

}
