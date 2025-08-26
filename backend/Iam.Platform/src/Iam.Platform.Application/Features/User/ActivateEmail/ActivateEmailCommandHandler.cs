using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ActivateEmail;

public class ActivateEmailCommandHandler (
    IKeycloakUserService keycloakUserService
) : IRequestHandler<ActivateEmailCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ActivateEmailCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            return ApiResponse<bool>.Fail("User ID cannot be null or empty.");
        }

        var success = await keycloakUserService.VerifyEmailAsync(command.UserId, cancellationToken);

        return success ? ApiResponse<bool>.Ok(true) : ApiResponse<bool>.Fail("Failed to activate email. User may not exist or activation failed.");
    }

}
