namespace MailService.Application.Interfaces;

public interface IAuthVerificationService
{
    /// <summary>
    /// Calls Iam.Platform to check if the token is fully valid.
    /// </summary>
    /// <param name="token">access token obtained from Keycloak</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token is valid, false if it's not</returns>
    Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);
}
