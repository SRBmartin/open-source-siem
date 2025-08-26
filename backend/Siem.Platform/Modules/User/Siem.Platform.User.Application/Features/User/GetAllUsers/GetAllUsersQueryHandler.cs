using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Application.DTOs.Identity.User.Retrieve;
using Siem.Platform.User.Application.DTOs.User;
using Siem.Platform.User.Application.Mappers.User;
using Siem.Platform.User.Domain.Repositories;

namespace Siem.Platform.User.Application.Features.User.GetAllUsers;

public class GetAllUsersQueryHandler (
    IUserRepository userRepository,
    IIdentityService identityService,
    ILogger<GetAllUsersQueryHandler> logger
) : IRequestHandler<GetAllUsersQuery, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var dbUsers = await userRepository.GetAllUsers(cancellationToken);

        var first = request.First ?? 0;
        var max = request.Max ?? 200;
        var iam = await identityService.GetIamUsersAsync(first, max, includeRoles: true, request.Search, cancellationToken);

        if (!iam.IsSuccess)
        {
            logger.LogWarning("IAM users fetch failed: {Error}", iam.Errors.FirstOrDefault()?.Message);
            return Result<List<UserDto>>.Failure(iam.Errors.ToArray());
        }

        var iamUsers = iam.Value ?? new List<IamUserListItemDto>();

        var rolesByEmail = iamUsers
            .Where(u => !string.IsNullOrWhiteSpace(u.Email))
            .GroupBy(u => u.Email!.Trim().ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.SelectMany(x => x.Roles ?? Array.Empty<string>())
                                           .Distinct(StringComparer.OrdinalIgnoreCase)
                                           .ToArray());

        var result = new List<UserDto>(dbUsers.Count);
        foreach (var u in dbUsers)
        {
            var emailKey = (u.Email ?? string.Empty).Trim().ToLowerInvariant();
            rolesByEmail.TryGetValue(emailKey, out var roles);
            result.Add(u.ToDto(roles ?? Array.Empty<string>()));
        }

        return Result<List<UserDto>>.Success(result);
    }
}
