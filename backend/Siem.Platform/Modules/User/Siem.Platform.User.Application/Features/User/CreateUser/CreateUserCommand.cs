using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Application.Features.User.CreateUser;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName
) : IRequest<Result<Guid>>;
