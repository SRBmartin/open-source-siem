using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Domain.Repositories;

namespace Siem.Platform.User.Application.Features.User.ModifyRole;

public class ModifyRoleCommandHandler (
    IIdentityService identityService,
    IUserRepository userRepository,
    ILogger<ModifyRoleCommandHandler> logger
) : IRequestHandler<ModifyRoleCommand, Result>
{
    public async Task<Result> Handle(ModifyRoleCommand command, CancellationToken cancellationToken)
    {
        if (string.Equals(command.InitiatorUserId, command.TargetUserId, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(new Error("authorization.self_edit_forbidden", "You cannot modify your own roles."));

        if (!Guid.TryParse(command.TargetUserId, out var targetGuid))
            return Result.Failure(new Error("validation.user_id.invalid", "Target user ID is invalid."));

        var targetUser = await userRepository.GetUserByUserId(targetGuid);
        if (targetUser is null)
            return Result.Failure(new Error("user.not_found", "Target user not found."));

        if (!targetUser.IsActivated)
            return Result.Failure(new Error("user.not_activated", "Target user is not activated."));

        var initiatorUser = await userRepository.GetUserByUserId(Guid.Parse(command.InitiatorUserId));
        if (initiatorUser is null)
            return Result.Failure(new Error("user.not_found", "Initiator user not found."));

        if (!initiatorUser.IsActivated)
            return Result.Failure(new Error("user.not_activated", "Initiator user is not activated."));

        var result = await identityService.ModifyUserRoleAsync(
            command.TargetUserId,
            command.Role,
            command.Action,
            cancellationToken
        );

        if (!result.IsSuccess)
            logger.LogWarning("Modify role failed. Initiator={Initiator} Target={Target} Role={Role} Action={Action} Error={Error}",
                command.InitiatorUserId, command.TargetUserId, command.Role, command.Action,
                result.Errors.FirstOrDefault()?.Message
            );

        return result;
    }

}
