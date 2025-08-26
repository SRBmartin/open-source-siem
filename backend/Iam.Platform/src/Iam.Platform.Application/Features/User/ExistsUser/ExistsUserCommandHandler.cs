using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ExistsUser;

public class ExistsUserCommandHandler (
    IKeycloakUserService keycloakUserService
) : IRequestHandler<ExistsUserCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(ExistsUserCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return ApiResponse<bool>.Fail("Email cannot be null or empty.");
        }

        var user = await keycloakUserService.GetUserByEmailAsync(command.Email, cancellationToken);
        return ApiResponse<bool>.Ok(user is not null);
    }
}
