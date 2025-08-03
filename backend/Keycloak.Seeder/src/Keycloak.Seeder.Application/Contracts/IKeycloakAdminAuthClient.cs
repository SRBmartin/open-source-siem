namespace Keycloak.Seeder.Application.Contracts;

public interface IKeycloakAdminAuthClient
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
