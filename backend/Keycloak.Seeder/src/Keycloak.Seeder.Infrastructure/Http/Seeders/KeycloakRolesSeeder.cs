using Keycloak.Seeder.Application.Contracts;
using Keycloak.Seeder.Domain.Seed;
using Keycloak.Seeder.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Keycloak.Seeder.Infrastructure.Http.Seeders;

public class KeycloakRolesSeeder (
    HttpClient httpClient,
    IKeycloakAdminAuthClient authClient,
    IOptions<KeycloakOptions> options,
    ILogger<KeycloakRolesSeeder> logger
) : IKeycloakRolesSeeder
{
    private readonly KeycloakOptions _options = options.Value;

    public async Task SeedRolesAsync(CancellationToken cancellationToken = default)
    {
        var token = await authClient.GetAccessTokenAsync(cancellationToken);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "roles.json");
        if (!File.Exists(filePath))
        {
            logger.LogWarning("\u001b[33mSeed file not found at: {Path}\u001b[0m", filePath);
            return;
        }

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var roles = JsonSerializer.Deserialize<List<Role>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];

        foreach (var role in roles)
        {
            if (await RoleExistsAsync(role.Name, cancellationToken))
            {
                logger.LogInformation("\u001b[34mRole '{Role}' already exists. Skipping.\u001b[0m", role.Name);
                continue;
            }

            var payload = JsonSerializer.Serialize(role, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var response = await httpClient.PostAsync(
                $"admin/realms/{_options.Realm}/roles",
                new StringContent(payload, System.Text.Encoding.UTF8, "application/json"),
                cancellationToken
            );

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("\u001b[32mCreated role '{Role}' successfully.\u001b[0m", role.Name);
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("\u001b[31mFailed to create role '{Role}': {Error}\u001b[0m", role.Name, err);
            }

        }

    }

    private async Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"admin/realms/{_options.Realm}/roles/{Uri.EscapeDataString(roleName)}",
            cancellationToken
        );

        return response.IsSuccessStatusCode;
    }
}
