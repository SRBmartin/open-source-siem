using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Domain.User;

namespace Iam.Platform.Application.Interfaces;

public interface IKeycloakUserService
{
    Task<string?> CreateUserAsync(CreateUserDto userDto, CancellationToken cancellationToken = default);
    Task<bool> VerifyEmailAsync(string userId, CancellationToken cancellationToken = default);
    Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<KeycloakUser?> GetUserByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string keycloakUserId, CancellationToken cancellationToken = default);
}
