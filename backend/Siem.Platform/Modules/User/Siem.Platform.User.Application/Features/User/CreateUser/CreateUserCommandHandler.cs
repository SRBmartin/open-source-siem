using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Domain.Repositories;
using Siem.Platform.User.Application.DTOs.Identity.User.Create;

namespace Siem.Platform.User.Application.Features.User.CreateUser;

public class CreateUserCommandHandler (
    IUserRepository userRepository,
    IIdentityService identityService,
    IRandomGenerator passwordGenerator
) : IRequestHandler<CreateUserCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var password = passwordGenerator.GenerateRandomPassword();

        var iamUserId = await identityService.CreateUserAsync(
            new CreateUserRequestDto(
                command.Email,
                command.FirstName,
                command.LastName,
                password
            )
        );

        if (!iamUserId.IsSuccess)
        {
            return Result<Guid>.Failure(iamUserId.Errors);
        }

        var user = Domain.Entities.User.Create(command.Email, command.FirstName, command.LastName, iamUserId.Value!.ExternalId);

        await userRepository.AddAsync(user, cancellationToken);

        //TODO: DIspatch email

        await userRepository.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(new(iamUserId.Value.ExternalId));
    }
}
