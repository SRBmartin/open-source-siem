using EBus.Abstractions;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.CreateUser;

public class CreateUserCommandHandler (
    IKeycloakUserService keycloakUserService,
    IUserValidationService userValidationService
) : IRequestHandler<CreateUserCommand, ApiResponse<CreateUserResponseDto>>
{
    public async Task<ApiResponse<CreateUserResponseDto>> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var validationErrors = userValidationService.ValidateCreateUser(command.User, cancellationToken);
        if (validationErrors is not null) return ApiResponse<CreateUserResponseDto>.Fail(validationErrors);

        var existingKeycioakUser = await keycloakUserService.GetUserByEmailAsync(command.User.Email, cancellationToken);
        if (existingKeycioakUser is not null) return ApiResponse<CreateUserResponseDto>.Fail("An user with that email already exists.");

        var keycloakUserId = await keycloakUserService.CreateUserAsync(command.User, cancellationToken);
        if (keycloakUserId is null) return ApiResponse<CreateUserResponseDto>.Fail("Failed to create user in policy engine.");

        return ApiResponse<CreateUserResponseDto>.Ok(new(keycloakUserId));
    }
}
