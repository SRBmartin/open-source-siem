namespace Iam.Platform.Application.Interfaces;

public interface IKeycloakTokenService
{
    /// <summary>
    /// Requests an access token using the client credentials grant type (from configuration).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token if success</returns>
    Task<string> GetClientCredentialsTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Used to retrieve user's access token with password grant type.
    /// </summary>
    /// <param name="username">Keycloak user's username (email)</param>
    /// <param name="password">Keycloak user's password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<string> GetPasswordTokenAsync(string username, string password, CancellationToken cancellationToken = default);

}
