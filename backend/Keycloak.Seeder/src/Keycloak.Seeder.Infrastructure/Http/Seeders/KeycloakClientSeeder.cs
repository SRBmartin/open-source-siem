using Keycloak.Seeder.Application.Contracts;
using Keycloak.Seeder.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net.Http.Headers;
using Keycloak.Seeder.Domain.Seed;

namespace Keycloak.Seeder.Infrastructure.Http.Seeders;

public class KeycloakClientSeeder (
    HttpClient httpClient,
    IKeycloakAdminAuthClient authClient,
    IOptions<KeycloakOptions> options,
    ILogger<KeycloakClientSeeder> logger
) : IKeycloakClientSeeder
{
    private readonly KeycloakOptions _options = options.Value;

    public async Task SeedClientsAsync(CancellationToken cancellationToken = default)
    {
        var token = await authClient.GetAccessTokenAsync(cancellationToken);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "clients.json");
        if (!File.Exists(filePath))
        {
            logger.LogWarning("\u001b[33mSeed file not found at: {Path}\u001b[0m", filePath);
            return;
        }

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var clients = JsonSerializer.Deserialize<List<Client>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];

        foreach (var client in clients)
        {
            var exists = await ClientExistsAsync(client.ClientId, cancellationToken);
            if (exists)
            {
                logger.LogInformation("\u001b[34mClient '{ClientId}' already exists. Skipping.\u001b[0m", client.ClientId);
                continue;
            }

            var payload = JsonSerializer.Serialize(client, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var response = await httpClient.PostAsync(
                $"admin/realms/{_options.Realm}/clients",
                new StringContent(payload, System.Text.Encoding.UTF8, "application/json"),
                cancellationToken
            );

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("\u001b[32mCreated client '{ClientId}' successfully.\u001b[0m", client.ClientId);
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("\u001b[31mFailed to create client '{ClientId}': {Error}\u001b[0m", client.ClientId, err);
            }
        }

    }

    private async Task<bool> ClientExistsAsync(string clientId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"admin/realms/{_options.Realm}/clients?clientId={clientId}",
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var arr = JsonDocument.Parse(content).RootElement;

        return arr.GetArrayLength() > 0;
    }

}
