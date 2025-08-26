using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Domain.Repositories;
using Siem.Platform.User.Application.DTOs.Identity.User.Create;
using Siem.Platform.Shared.Application.Abstractions.Services;
using Siem.Platform.Shared.Application.Abstractions.Common.DTOs;
using Microsoft.Extensions.Logging;

namespace Siem.Platform.User.Application.Features.User.CreateUser;

public class CreateUserCommandHandler (
    IUserRepository userRepository,
    IIdentityService identityService,
    IRandomGenerator randomGenerator,
    IMailGateway mailService,
    ILogger<CreateUserCommandHandler> logger
) : IRequestHandler<CreateUserCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var dbExists = await userRepository.ExistsByEmailAsync(command.Email, cancellationToken);
        var iamExists = await identityService.ExistsByEmailAsync(command.Email, cancellationToken);

        if (dbExists || iamExists.Value)
        {
            return Result<Guid>.Failure(new Error("user.already_exists", $"User with email {command.Email} already exists."));
        }

        var password = randomGenerator.GenerateRandomPassword();

        var iamUserId = await identityService.CreateUserAsync(
            new CreateUserRequestDto(
                command.Email,
                command.FirstName,
                command.LastName,
                password
            )
        );

        if (!iamUserId.IsSuccess)
        {
            return Result<Guid>.Failure(iamUserId.Errors);
        }

        var user = Domain.Entities.User.Create(command.Email, command.FirstName, command.LastName, iamUserId.Value!.ExternalId);

        var activationToken = randomGenerator.GenerateUriSafeToken(48);
        var hashedActivationToken = randomGenerator.HashTokenSha256(activationToken);
        var token = user.IssueActivationToken(hashedActivationToken);

        try
        {
            await userRepository.AddAsync(user, cancellationToken);
            await userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create user {Email} with ID {Id}.", command.Email, iamUserId.Value!.ExternalId);

            await identityService.DeleteUserAsync(iamUserId.Value!.ExternalId, cancellationToken);
            return Result<Guid>.Failure(new Error("user.create_failed", $"Failed to create user: {ex.Message}"));
        }

        var emailRequest = new EmailVerifyRequestDto
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            Token = activationToken,
            Password = password
        };

        try
        {
            await mailService.SendVerificationEmailAsync(emailRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send verification email for user {Email} with ID {Id}.", command.Email, iamUserId.Value!.ExternalId);

            try
            {
                //Hard delete, because it shouldn't be in the system if something fails, like it has never existed
                userRepository.Delete(user, soft: false);
                await userRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception dex)
            {
                logger.LogError(dex, "Failed to delete user {Email} with ID {Id} after email sending failure.", command.Email, iamUserId.Value!.ExternalId);
            }

            await identityService.DeleteUserAsync(iamUserId.Value!.ExternalId, cancellationToken);
            return Result<Guid>.Failure(new Error("user.email_send_failed", $"Failed to send verification email: {ex.Message}"));
        }

        return Result<Guid>.Success(new(iamUserId.Value.ExternalId));
    }

}
