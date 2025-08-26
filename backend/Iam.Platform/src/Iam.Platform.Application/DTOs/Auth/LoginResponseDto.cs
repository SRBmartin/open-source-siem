namespace Iam.Platform.Application.DTOs.Auth;

public sealed class LoginResponseDto
{
    public string AccessToken { get; init; } = default!;
}
