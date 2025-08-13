namespace Siem.Platform.Shared.Application.Abstractions.Services;

public interface IKeycloakTokenService
{
    /// <summary>
    /// Requests an access token using the client credentials grant type (from configuration).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token if success</returns>
    Task<string> GetClientCredentialsTokenAsync(CancellationToken cancellationToken = default);
}
