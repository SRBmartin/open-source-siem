using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler (
    IKeycloakUserService keycloakUserService    
) : IRequestHandler<LogoutCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            return ApiResponse<bool>.Fail("User ID must be provided.");
        }

        try
        {
            var ok = await keycloakUserService.LogoutUserSessionsAsync(command.UserId, cancellationToken);
            return ok ? ApiResponse<bool>.Ok(true) : ApiResponse<bool>.Fail("Failed to terminate user sessions.");
        }
        catch (Exception)
        {
            return ApiResponse<bool>.Fail("Logout failed due to an unexpected error.");
        }

    }
}
