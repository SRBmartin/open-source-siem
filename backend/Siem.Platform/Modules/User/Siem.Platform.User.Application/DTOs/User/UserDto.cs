namespace Siem.Platform.User.Application.DTOs.User;

public sealed record UserDto
(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsActivated,
    IReadOnlyList<string> Roles
);