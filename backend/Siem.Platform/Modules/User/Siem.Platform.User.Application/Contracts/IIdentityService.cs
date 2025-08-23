using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.DTOs.Identity.User.Create;

namespace Siem.Platform.User.Application.Contracts;

public interface IIdentityService
{
    Task<Result<CreateUserResponseDto>> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}
