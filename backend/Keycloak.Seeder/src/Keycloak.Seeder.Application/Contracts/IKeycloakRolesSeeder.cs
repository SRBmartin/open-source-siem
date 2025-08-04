namespace Keycloak.Seeder.Application.Contracts;

public interface IKeycloakRolesSeeder
{
    Task SeedRolesAsync(CancellationToken cancellationToken = default);
}
