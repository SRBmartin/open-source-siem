namespace Iam.Platform.Application.DTOs.User;

public sealed record ModifyUserRoleRequestDto
(
    string Role,
    string Action
);
