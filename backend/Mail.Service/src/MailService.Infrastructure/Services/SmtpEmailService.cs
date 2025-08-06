using MailKit.Security;
using MailService.Application.Interfaces;
using MailService.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using RazorLight;
using MailKit.Net.Smtp;

namespace MailService.Infrastructure.Services;

public class SmtpEmailService(
    IOptions<MailSettings> options,
    IRazorLightEngine razor,
    IAsyncPolicy retryPolicy
) : IEmailService
{
    private readonly MailSettings _settings = options.Value;

    public async Task SendEmailAsync<TModel>(string to, string subject, string templateName, TModel model, CancellationToken cancellationToken = default)
    {
        string html = await razor.CompileRenderAsync($"{templateName}.cshtml", model);

        await retryPolicy.ExecuteAsync(async ct =>
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = html };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
              _settings.SmtpServer,
              _settings.Port,
              SecureSocketOptions.SslOnConnect,
            ct);
            await client.AuthenticateAsync(
              _settings.Username,
              _settings.Password,
            ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }, cancellationToken);
    }
}
