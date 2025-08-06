namespace Iam.Platform.Infrastructure.Identity.Models;

public class KeycloakCreateUserRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool Enabled { get; set; } = true;
    public bool EmailVerified { get; set; } = false;
    public List<KeycloakUserCredential> Credentials { get; set; } = new();
}
