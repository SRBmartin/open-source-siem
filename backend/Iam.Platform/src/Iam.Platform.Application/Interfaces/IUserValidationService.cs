using Iam.Platform.Application.DTOs.User;

namespace Iam.Platform.Application.Interfaces;

public interface IUserValidationService
{
    string? ValidateCreateUser(CreateUserDto userDto, CancellationToken cancellationToken = default);
}
