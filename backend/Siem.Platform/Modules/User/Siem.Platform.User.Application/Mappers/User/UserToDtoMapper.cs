using Siem.Platform.User.Application.DTOs.User;

namespace Siem.Platform.User.Application.Mappers.User;

public static class UserToDtoMapper
{
    public static UserDto ToDto(this Domain.Entities.User user, IEnumerable<string> roles)
        => new(
            UserId: user.Id.ToString(),
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            IsActivated: user.IsActivated,
            Roles: roles?.Where(r => !string.IsNullOrWhiteSpace(r))
                         .Select(r => r.Trim())
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .OrderBy(r => r, StringComparer.OrdinalIgnoreCase)
                         .ToList() ?? new List<string>()
        );
}
