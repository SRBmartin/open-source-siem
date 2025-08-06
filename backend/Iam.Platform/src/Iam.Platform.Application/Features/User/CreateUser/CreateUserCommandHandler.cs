using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.CreateUser;

public class CreateUserCommandHandler (
    IKeycloakUserService keycloakUserService,
    IUserValidationService userValidationService
) : IRequestHandler<CreateUserCommand, ApiResponse<string>>
{
    public async Task<ApiResponse<string>> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var validationErrors = userValidationService.ValidateCreateUser(command.User, cancellationToken);
        if (validationErrors is not null) return ApiResponse<string>.Fail(validationErrors);

        var existingKeycioakUser = await keycloakUserService.GetUserByEmailAsync(command.User.Email, cancellationToken);
        if (existingKeycioakUser is not null) return ApiResponse<string>.Fail("An user with that email already exists.");

        var keycloakUserId = await keycloakUserService.CreateUserAsync(command.User, cancellationToken);
        if (keycloakUserId is null) return ApiResponse<string>.Fail("Failed to create user in policy engine.");

        //TODO: Dispatch an event to create a user in user mgmt microservice

        return ApiResponse<string>.Ok("User was successfuly created. Please check email to confirm account.");
    }
}
