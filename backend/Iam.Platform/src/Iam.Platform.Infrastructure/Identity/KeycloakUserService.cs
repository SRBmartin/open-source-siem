using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Infrastructure.Configuration;
using Iam.Platform.Infrastructure.Identity.Mappers;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net;
using Iam.Platform.Domain.User;
using System.Net.Http.Json;
using Iam.Platform.Infrastructure.Identity.Models;

namespace Iam.Platform.Infrastructure.Identity;

public class KeycloakUserService (
    HttpClient httpClient,
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

    public async Task<bool> VerifyEmailAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByUserIdAsync(userId, cancellationToken);
        if (user is null) return false;

        user.EmailVerified = true;

        var response = await httpClient.PutAsJsonAsync($"/admin/realms/{_options.Realm}/users/{userId}", user, cancellationToken);

        return response.IsSuccessStatusCode;
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

    public async Task<KeycloakUser?> GetUserByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/admin/realms/{_options.Realm}/users/{userId}", cancellationToken);

        if (!response.IsSuccessStatusCode) return null!;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var user = JsonSerializer.Deserialize<KeycloakUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return user ?? null!;
    }

    public async Task<bool> DeleteUserAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(
            $"/admin/realms/{_options.Realm}/users/{keycloakUserId}",
            cancellationToken
        );

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> LogoutUserSessionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var resp = await httpClient.PostAsync(
            $"/admin/realms/{_options.Realm}/users/{userId}/logout",
            content: null,
            cancellationToken);

        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> SetUserPasswordAsync(string userId, string password, CancellationToken cancellationToken = default)
    {
        var payload = new KeycloakCredential { Value = password };

        var response = await httpClient.PutAsJsonAsync($"/admin/realms/{_options.Realm}/users/{userId}/reset-password", payload, cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<KeycloakRole?> GetRealmRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var resp = await httpClient.GetAsync(
            $"/admin/realms/{_options.Realm}/roles/{Uri.EscapeDataString(roleName)}",
            cancellationToken);

        if (!resp.IsSuccessStatusCode)
            return null;

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<KeycloakRole>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<IReadOnlyList<KeycloakRole>> GetUserRealmRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var resp = await httpClient.GetAsync(
            $"/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm",
            cancellationToken);

        if (!resp.IsSuccessStatusCode)
            return Array.Empty<KeycloakRole>();

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        var roles = JsonSerializer.Deserialize<List<KeycloakRole>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return roles ?? new List<KeycloakRole>();
    }

    public async Task<bool> AddRealmRolesToUserAsync(string userId, IEnumerable<KeycloakRole> roles, CancellationToken cancellationToken = default)
    {
        var resp = await httpClient.PostAsJsonAsync(
            $"/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm",
            roles,
            cancellationToken);

        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveRealmRolesFromUserAsync(string userId, IEnumerable<KeycloakRole> roles, CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm")
        {
            Content = JsonContent.Create(roles)
        };

        var resp = await httpClient.SendAsync(req, cancellationToken);
        return resp.IsSuccessStatusCode;
    }

    public async Task<IReadOnlyList<KeycloakUser>> GetUsersAsync(int first = 0, int max = 50, string? search = null, CancellationToken cancellationToken = default)
    {
        var q = new List<string>();
        if (first > 0) q.Add($"first={first}");
        if (max > 0) q.Add($"max={max}");
        if (!string.IsNullOrWhiteSpace(search)) q.Add($"search={Uri.EscapeDataString(search)}");

        var url = $"/admin/realms/{_options.Realm}/users";
        if (q.Count > 0) url += "?" + string.Join("&", q);

        var resp = await httpClient.GetAsync(url, cancellationToken);
        if (!resp.IsSuccessStatusCode)
            return Array.Empty<KeycloakUser>();

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonSerializer.Deserialize<List<KeycloakUser>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return users ?? new List<KeycloakUser>();
    }
}
