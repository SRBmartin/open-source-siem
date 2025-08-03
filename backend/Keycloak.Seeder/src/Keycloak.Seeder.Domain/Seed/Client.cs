namespace Keycloak.Seeder.Domain.Seed;

public class Client
{
    public string ClientId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool PublicClient { get; set; } = false;
    public bool DirectAccessGrantsEnabled { get; set; } = false;
    public List<string>? RedirectUris { get; set; }
}
