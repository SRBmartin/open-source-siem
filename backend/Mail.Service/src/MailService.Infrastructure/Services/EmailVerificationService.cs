using MailService.Application.Interfaces;
using MailService.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace MailService.Infrastructure.Services;

public class EmailVerificationService (
        IEmailService emailService,
        IOptions<UrisSettings> options
) : IEmailVerificationService
{
    private readonly UrisSettings _settings = options.Value;

    public Task SendVerificationEmailAsync(string userId, string email, string token, CancellationToken cancellationToken = default)
    {
        var activationLink = $"{_settings.FrontendUri.TrimEnd('/')}/{userId}/{token}";

        var model = new
        {
            ActivationLink = activationLink
        };

        return emailService.SendEmailAsync(
            to: email,
            subject: "[SIEM]: Verify your email address",
            templateName: "VerifyEmailMail",
            model: model,
            cancellationToken: cancellationToken
        );
    }
}