using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Domain.Repositories;

namespace Siem.Platform.User.Application.Features.Auth.LoginCommand;

public sealed class LoginCommandHandler (
    IIdentityService identityService,
    IUserRepository userRepository,
    ILogger<LoginCommandHandler> logger
) : IRequestHandler<LoginCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Username, cancellationToken);
        if (user is null)
        {
            logger.LogDebug("Login failed for user {Username}: User not found", command.Username);
            return Result<string>.Failure(new Error("auth.user_not_found", "Please check your credentials and try again."));
        }

        if (!user.IsActivated)
        {
            return Result<string>.Failure(new Error("auth.user_not_activated", "User account is not activated. Please check your email for the activation link."));
        }

        var accessToken = await identityService.LoginAsync(command.Username, command.Password, cancellationToken);

        if (!accessToken.IsSuccess)
        {
            logger.LogDebug("Login failed for user {Username}: {Message}", command.Username, accessToken.Errors.FirstOrDefault()?.Message);
            return Result<string>.Failure(new Error("auth.login_failed", "Login failed. Please check your credentials and try again."));
        }

        return Result<string>.Success(accessToken.Value!);
    }

}
