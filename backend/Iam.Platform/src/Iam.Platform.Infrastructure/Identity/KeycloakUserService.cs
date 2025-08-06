using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Infrastructure.Configuration;
using Iam.Platform.Infrastructure.Identity.Mappers;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net;
using Iam.Platform.Domain.User;
using System.Net.Http.Json;

namespace Iam.Platform.Infrastructure.Identity;

public class KeycloakUserService (
    HttpClient httpClient,
    IKeycloakTokenService keycloakTokenService,
    IOptions<KeycloakSettings> keycloakOptions
) : IKeycloakUserService
{
    private readonly KeycloakSettings _options = keycloakOptions.Value;

    public async Task<string?> CreateUserAsync(CreateUserDto userDto, CancellationToken cancellationToken = default)
    {
        var payload = userDto.ToKeycloakRequest();

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.BaseUrl}/admin/realms/{_options.Realm}/users"
        )
        {
            Content = JsonContent.Create(payload)
        };

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict) return null!;
        if (!response.IsSuccessStatusCode) return null!;

        var id = response.Headers.Location?.Segments.LastOrDefault()?.TrimEnd('/');

        return string.IsNullOrWhiteSpace(id) ? null! : id;
    }

    public async Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(
            $"/admin/realms/{_options.Realm}/users?email={Uri.EscapeDataString(email)}",
            cancellationToken
        );

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonSerializer.Deserialize<List<KeycloakUser>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return users?.FirstOrDefault() ?? null!;
    }

    public async Task<bool> DeleteUserAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(
            $"/admin/realms/{_options.Realm}/users/{keycloakUserId}",
            cancellationToken
        );

        return response.IsSuccessStatusCode;
    }

}
