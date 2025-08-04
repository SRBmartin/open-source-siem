using Keycloak.Seeder.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Keycloak.Seeder.Application.Services;

public class SeederWorker (
    IKeycloakClientSeeder clientSeeder,
    IKeycloakRolesSeeder rolesSeeder,
    ILogger<SeederWorker> logger
) : ISeederWorker
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("\u001b[36m--- Starting Keycloak Seeding ---\u001b[0m");

        await clientSeeder.SeedClientsAsync(cancellationToken);
        await rolesSeeder.SeedRolesAsync(cancellationToken);

        logger.LogInformation("\u001b[36m--- Keycloak Seeding Completed ---\u001b[0m");
    }
}
