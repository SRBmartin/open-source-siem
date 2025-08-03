namespace Keycloak.Seeder.Application.Contracts;

public interface IKeycloakClientSeeder
{
    Task SeedClientsAsync(CancellationToken cancellationToken = default);
}
