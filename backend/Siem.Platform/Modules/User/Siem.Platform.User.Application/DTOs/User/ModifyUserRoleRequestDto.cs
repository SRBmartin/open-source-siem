namespace Siem.Platform.User.Application.DTOs.User;

public sealed record ModifyUserRoleRequestDto
(
    string TargetUserId,
    string Role,
    string Action //"add" or "remove"
);