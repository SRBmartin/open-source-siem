using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Application.Features.User.ModifyRole;

public sealed record ModifyRoleCommand
(
    string InitiatorUserId, // from JWT
    string TargetUserId,    // from request body
    string Role,            // no hardcoded constraints, but should be a valid role name
    string Action           // "add" | "remove"
) : IRequest<Result>;
