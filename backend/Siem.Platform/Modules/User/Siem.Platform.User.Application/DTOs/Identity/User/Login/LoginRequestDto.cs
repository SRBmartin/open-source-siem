namespace Siem.Platform.User.Application.DTOs.Identity.User.Login;

public sealed class LoginRequestDto
{
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
