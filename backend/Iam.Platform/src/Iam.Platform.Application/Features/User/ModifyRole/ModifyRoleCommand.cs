using EBus.Abstractions;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ModifyRole;

public sealed record ModifyRoleCommand
(
    string UserId,
    string Role,
    string Action
) : IRequest<ApiResponse<bool>>;
