using Keycloak.Seeder.Application.Contracts;
using Keycloak.Seeder.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Keycloak.Seeder.Infrastructure.Http;

public class KeycloakAdminAuthClient (
    HttpClient httpClient,
    IOptions<KeycloakOptions> options
) : IKeycloakAdminAuthClient
{
    private readonly KeycloakOptions _options = options.Value;
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
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
}
