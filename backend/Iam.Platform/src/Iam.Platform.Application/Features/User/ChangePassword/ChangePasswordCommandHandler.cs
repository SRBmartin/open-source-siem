using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ChangePassword;

public sealed class ChangePasswordCommandHandler (
    IKeycloakUserService keycloakUserService,
    IKeycloakTokenService keycloakTokenService
) : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
            return ApiResponse<bool>.Fail("User ID is required.");

        if (string.IsNullOrWhiteSpace(command.CurrentPassword) || string.IsNullOrWhiteSpace(command.NewPassword))
            return ApiResponse<bool>.Fail("Current and new password must be provided.");

        var kcUser = await keycloakUserService.GetUserByUserIdAsync(command.UserId, cancellationToken);
        if (kcUser is null)
            return ApiResponse<bool>.Fail("User not found.");

        var username = !string.IsNullOrWhiteSpace(kcUser.Username) ? kcUser.Username : kcUser.Email;

        if (string.IsNullOrWhiteSpace(username))
            return ApiResponse<bool>.Fail("User has no valid username.");

        try
        {
            _ = await keycloakTokenService.GetPasswordTokenAsync(username, command.CurrentPassword, cancellationToken);
        }
        catch
        {
            return ApiResponse<bool>.Fail("Incorrect password. Check credentials and try again.");
        }

        var success = await keycloakUserService.SetUserPasswordAsync(command.UserId, command.NewPassword, cancellationToken);

        return success ? ApiResponse<bool>.Ok(true) : ApiResponse<bool>.Fail("Failed to change password.");
    }

}
