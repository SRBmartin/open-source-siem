using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;

namespace Siem.Platform.User.Application.Features.Auth.LogoutCommand;

public class LogoutCommandHandler (
    IIdentityService identityService,
    ILogger<LogoutCommandHandler> logger
) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var response = await identityService.LogoutAsync(command.UserId, cancellationToken);

        if (!response.IsSuccess)
        {
            logger.LogWarning("Logout failed for user {UserId}: {Message}", command.UserId, response.Errors.FirstOrDefault()?.Message);
            return Result.Failure(response.Errors);
        }

        return Result.Success();
    }

}
