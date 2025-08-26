namespace Iam.Platform.Infrastructure.Identity.Models;

public class KeycloakUserCredential
{
    public required string Type { get; set; }
    public required string Value { get; set; }
    public bool Temporary { get; set; } = false;
}
