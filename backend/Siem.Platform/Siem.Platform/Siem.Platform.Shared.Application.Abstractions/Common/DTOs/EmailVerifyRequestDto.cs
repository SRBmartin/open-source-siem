namespace Siem.Platform.Shared.Application.Abstractions.Common.DTOs;

public class EmailVerifyRequestDto
{
    public string UserId { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Token { get; init; } = default!;
    public string Password { get; init; } = default!;
}
