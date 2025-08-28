using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ModifyRole;

public class ModifyRoleCommandHandler (
    IKeycloakUserService keycloakUserService
) : IRequestHandler<ModifyRoleCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ModifyRoleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
            return ApiResponse<bool>.Fail("User ID is required.");

        if (string.IsNullOrWhiteSpace(command.Role))
            return ApiResponse<bool>.Fail("Role is required.");

        if (string.IsNullOrWhiteSpace(command.Action))
            return ApiResponse<bool>.Fail("Action is required. Allowed values: 'add' or 'remove'.");

        var action = command.Action.Trim().ToLowerInvariant();
        if (action is not ("add" or "remove"))
            return ApiResponse<bool>.Fail("Invalid action. Allowed values: 'add' or 'remove'.");

        var user = await keycloakUserService.GetUserByUserIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return ApiResponse<bool>.Fail("User not found.");

        var targetRole = await keycloakUserService.GetRealmRoleByNameAsync(command.Role, cancellationToken);
        if (targetRole is null)
            return ApiResponse<bool>.Fail($"Role '{command.Role}' was not found in the realm.");

        var userRoles = await keycloakUserService.GetUserRealmRolesAsync(command.UserId, cancellationToken);
        var hasRole = userRoles.Any(t => string.Equals(t.Name, targetRole.Name, StringComparison.OrdinalIgnoreCase));

        if (action == "add")
        {
            if (hasRole)
                return ApiResponse<bool>.Ok(true);

            var ok = await keycloakUserService.AddRealmRolesToUserAsync(command.UserId, new[] { targetRole }, cancellationToken);
            return ok
                ? ApiResponse<bool>.Ok(true)
                : ApiResponse<bool>.Fail($"Failed to add role '{targetRole.Name}' to the user.");
        }
        else
        {
            if (!hasRole)
                return ApiResponse<bool>.Ok(true);

            var ok = await keycloakUserService.RemoveRealmRolesFromUserAsync(command.UserId, new[] { targetRole }, cancellationToken);
            return ok
                ? ApiResponse<bool>.Ok(true)
                : ApiResponse<bool>.Fail($"Failed to remove role '{targetRole.Name}' from the user.");
        }

    }

}
