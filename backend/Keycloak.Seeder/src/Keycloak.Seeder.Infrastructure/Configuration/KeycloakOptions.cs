namespace Keycloak.Seeder.Infrastructure.Configuration;

public class KeycloakOptions
{
    public string BaseUrl { get; set; } = default!;
    public string Realm { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}