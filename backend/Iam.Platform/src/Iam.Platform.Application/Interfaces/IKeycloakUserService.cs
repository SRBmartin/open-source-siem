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

    /// <summary>
    /// Logs out all sessions of the user with the provided userId (Admin logout).
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<bool> LogoutUserSessionsAsync(string userId, CancellationToken cancellationToken = default);
}
