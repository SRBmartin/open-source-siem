namespace Siem.Platform.User.Application.DTOs.Identity.User.Retrieve;

public sealed record IamUserListItemDto(
    string? Id,
    string? Username,
    string? Email,
    string? FirstName,
    string? LastName,
    bool? EmailVerified,
    bool? Enabled,
    IReadOnlyList<string>? Roles
);