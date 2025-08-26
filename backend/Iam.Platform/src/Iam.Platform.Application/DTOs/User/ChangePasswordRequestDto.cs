namespace Iam.Platform.Application.DTOs.User;

public sealed record ChangePasswordRequestDto (
    string CurrentPassword,
    string NewPassword
);
