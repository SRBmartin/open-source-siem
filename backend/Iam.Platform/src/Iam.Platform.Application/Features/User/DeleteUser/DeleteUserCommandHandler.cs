using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.DeleteUser;

public class DeleteUserCommandHandler (
    IKeycloakUserService keycloakUserService
) : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            return ApiResponse<bool>.Fail("User ID cannot be null or empty.");
        }

        var success = await keycloakUserService.DeleteUserAsync(command.UserId, cancellationToken);

        return success ? ApiResponse<bool>.Ok(true) : ApiResponse<bool>.Fail("Failed to delete user. User may not exist or deletion failed.");
    }
}
