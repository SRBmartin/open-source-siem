using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Domain.Repositories;

namespace Siem.Platform.User.Application.Features.User.ChangePassword;

public class ChangePasswordCommandHandler (
    IIdentityService identityService,
    IUserRepository userRepository,
    ILogger<ChangePasswordCommandHandler> logger
) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUserId(Guid.Parse(command.UserId));
        if (user is null)
        {
            return Result.Failure(new Error("user.not_found", "User not found."));
        }
        if (!user.IsActivated)
        {
            return Result.Failure(new Error("user.not_activated", "User account is not activated. Please check your email for the activation link."));
        }

        var result = await identityService.ChangePasswordAsync(
            command.UserId,
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken
        );

        if (!result.IsSuccess)
        {
            logger.LogWarning("Password change failed for user {UserId}: {Message}", command.UserId, result.Errors.FirstOrDefault()?.Message);
        }

        return result;
    }

}
