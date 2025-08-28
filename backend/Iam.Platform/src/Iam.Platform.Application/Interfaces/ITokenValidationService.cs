namespace Iam.Platform.Application.Interfaces;

public interface ITokenValidationService
{
    /// <summary>
    /// Validates an access token by calling Keycloak’s introspection endpoint.
    /// Returns a ClaimsPrincipal if valid, or null if not active.
    /// </summary>
    /// <param name="token">access_token from clients_credentials</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
