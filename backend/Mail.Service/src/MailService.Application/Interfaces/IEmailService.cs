namespace MailService.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync<TModel>(string to, string subject, string templateName, TModel model, CancellationToken cancellationToken = default);
}
