namespace Siem.Platform.User.Application.DTOs.Identity.User.Password;

public sealed class ChangePasswordRequestDto
{
    public string CurrentPassword { get; init; } = default!;
    public string NewPassword { get; init; } = default!;
}
