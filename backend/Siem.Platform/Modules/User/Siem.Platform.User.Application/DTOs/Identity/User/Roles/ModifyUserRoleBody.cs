namespace Siem.Platform.User.Application.DTOs.Identity.User.Roles;

public sealed record ModifyUserRoleBody
(
    string Role,
    string Action
);