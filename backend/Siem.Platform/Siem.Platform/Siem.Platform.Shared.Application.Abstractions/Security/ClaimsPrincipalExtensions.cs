using System.Security.Claims;

namespace Siem.Platform.Shared.Application.Abstractions.Security;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? user.FindFirst("sub")?.Value;

}
