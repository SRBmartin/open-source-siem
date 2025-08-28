namespace Siem.Platform.User.Application.DTOs.User;

public class ChangePasswordRequestDto
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
