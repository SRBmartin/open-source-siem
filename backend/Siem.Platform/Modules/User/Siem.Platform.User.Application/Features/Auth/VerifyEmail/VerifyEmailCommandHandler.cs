using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Domain.Repositories;

namespace Siem.Platform.User.Application.Features.Auth.VerifyEmail;

public class VerifyEmailCommandHandler (
    IIdentityService identityService,
    IActivationTokenRepository activationTokenRepository,
    IUserRepository userRepository,
    IRandomGenerator randomGenerator,
    ILogger<VerifyEmailCommandHandler> logger
) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(command.UserId);
        var hashedToken = randomGenerator.HashTokenSha256(command.ActivationToken);

        var token = await activationTokenRepository.GetActiveByUserAndHashAsync(userId, hashedToken, cancellationToken);
        if (token is null)
        {
            return Result.Failure(new Error("activation.not_found_or_expired", "Activation token not found or expired."));
        }

        var iamResponse = await identityService.VerifyEmailAsync(userId.ToString(), cancellationToken);
        if (!iamResponse.IsSuccess)
        {
            logger.LogWarning("IAM verify email failed for user {UserId}: {Message}", command.UserId, iamResponse.Errors.FirstOrDefault()?.Message);
            return Result.Failure(iamResponse.Errors);
        }

        var user = await userRepository.GetUserByUserId(userId, cancellationToken);

        user.MarkActivated();
        token.MarkUsed();

        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
