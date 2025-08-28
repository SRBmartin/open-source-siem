using EBus.Abstractions;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;
using Iam.Platform.Domain.User;

namespace Iam.Platform.Application.Features.User.GetUsers;

public sealed class GetUsersQueryHandler (
    IKeycloakUserService keycloakUserService
) : IRequestHandler<GetUsersQuery, ApiResponse<List<UserListItemDto>>>
{
    public async Task<ApiResponse<List<UserListItemDto>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        if (query.Max is < 1 or > 200)
            return ApiResponse<List<UserListItemDto>>.Fail("Parameter 'max' must be between 1 and 200.");

        var users = await keycloakUserService.GetUsersAsync(
            first: query.First,
            max: query.Max,
            search: query.Search,
            cancellationToken: cancellationToken
        );

        if (users is null || users.Count == 0)
            return ApiResponse<List<UserListItemDto>>.Ok(new List<UserListItemDto>());

        IReadOnlyList<IReadOnlyList<KeycloakRole>> rolesPerUser;
        if (query.IncludeRoles)
        {
            var tasks = users.Select(u => keycloakUserService.GetUserRealmRolesAsync(u.Id!, cancellationToken));
            var results = await Task.WhenAll(tasks);
            rolesPerUser = results;
        }
        else
        {
            rolesPerUser = users.Select(_ => Array.Empty<KeycloakRole>()).ToList();
        }

        var dtos = new List<UserListItemDto>(users.Count);
        for (int i = 0; i < users.Count; i++)
        {
            var u = users[i];
            var roles = rolesPerUser[i].Select(r => r.Name!).Where(n => !string.IsNullOrWhiteSpace(n))
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                               .ToList();

            dtos.Add(new UserListItemDto(
                Id: u.Id!,
                Username: u.Username,
                Email: u.Email,
                FirstName: u.FirstName,
                LastName: u.LastName,
                EmailVerified: u.EmailVerified,
                Enabled: u.Enabled,
                Roles: roles));
        }

        return ApiResponse<List<UserListItemDto>>.Ok(dtos);
    }
}
