using Iam.Platform.Application.Interfaces;
using Iam.Platform.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Iam.Platform.Infrastructure.Identity;

public class KeycloakTokenValidationService (
    HttpClient httpClient,
    IOptions<KeycloakSettings> options
) : ITokenValidationService
{
    private readonly KeycloakSettings _settings = options.Value;

    /// <inheritdoc/>
    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var creds = $"{_settings.ClientId}:{_settings.ClientSecret}";
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes(creds));
        
        using var req = new HttpRequestMessage(HttpMethod.Post,
            $"/realms/{_settings.Realm}/protocol/openid-connect/token/introspect");
        req.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", basic);

        req.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string,string>("token", token),
            new("token_type_hint","access_token")
        });

        Console.WriteLine($"{_settings.BaseUrl}/{_settings.Realm}");
        Console.WriteLine($"{token}");

        var resp = await httpClient.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode) return false;

        Console.WriteLine($"Content: {await resp.Content.ReadAsStringAsync(cancellationToken)}");

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(cancellationToken));
        
        return doc.RootElement.GetProperty("active").GetBoolean();
    }
}
