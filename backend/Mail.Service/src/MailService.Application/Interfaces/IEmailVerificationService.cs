namespace MailService.Application.Interfaces;

public interface IEmailVerificationService
{
    /// <param name="userId">The id of the user to embed in the link.</param>
    /// <param name="email">The recipient email address.</param>
    /// <param name="token">The activation token to append.</param>
    Task SendVerificationEmailAsync(string userId, string email, string token, CancellationToken cancellationToken = default);
}
