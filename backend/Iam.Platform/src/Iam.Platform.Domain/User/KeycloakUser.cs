namespace Iam.Platform.Domain.User;

public class KeycloakUser
{
    public string Id { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool Enabled { get; set; }
    public bool EmailVerified { get; set; }
}
