namespace Iam.Platform.Application.DTOs.Auth;

public sealed class LoginRequestDto
{
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
