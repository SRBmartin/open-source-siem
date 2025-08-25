namespace Iam.Platform.Application.DTOs.User;

public sealed record UserListItemDto(
    string Id,
    string? Username,
    string? Email,
    string? FirstName,
    string? LastName,
    bool? EmailVerified,
    bool? Enabled,
    IReadOnlyList<string> Roles
);