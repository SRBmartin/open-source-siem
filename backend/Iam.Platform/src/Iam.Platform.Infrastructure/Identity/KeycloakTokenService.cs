using Iam.Platform.Application.Interfaces;
using Iam.Platform.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Iam.Platform.Infrastructure.Identity;

public class KeycloakTokenService (
    HttpClient httpClient,
    IOptions<KeycloakSettings> keycloakSettings
) : IKeycloakTokenService
{
    private readonly KeycloakSettings _options = keycloakSettings.Value;

    public async Task<string> GetClientCredentialsTokenAsync(CancellationToken cancellationToken = default)
    {
        var form = new Dictionary<string, string>()
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["grant_type"] = "client_credentials"
        };

        var response = await httpClient.PostAsync(
            $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("access_token").GetString() ??
            throw new InvalidDataException("There is no access_token provided.");
    }

    public async Task<string> GetPasswordTokenAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var form = new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["grant_type"] = "password",
            ["username"] = username,
            ["password"] = password
        };

        var response = await httpClient.PostAsync(
            $"{_options.BaseUrl}/realms/{_options.Realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidDataException("No access_token provided.");
    }

}
