using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Siem.Platform.Shared.Application.Abstractions.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Siem.Platform.User.Api.Security;

public sealed class BearerTokenFilter(
    IAuthVerificationService authService
) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var header = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = header["Bearer ".Length..].Trim();
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!(await authService.VerifyTokenAsync(token, context.HttpContext.RequestAborted)))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jwt = null;
        try { jwt = handler.ReadJwtToken(token); }
        catch { /* if it isn't a JWT, continue without claims */ }

        if (jwt is not null)
        {
            var claims = new List<Claim>(jwt.Claims);

            var sub = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (!string.IsNullOrEmpty(sub) && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
            }

            var realmAccess = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            if (!string.IsNullOrWhiteSpace(realmAccess))
            {
                try
                {
                    using var doc = JsonDocument.Parse(realmAccess);
                    if (doc.RootElement.TryGetProperty("roles", out var rolesElement) &&
                        rolesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var r in rolesElement.EnumerateArray())
                        {
                            var role = r.GetString();
                            if (!string.IsNullOrWhiteSpace(role))
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role!));
                            }
                        }
                    }
                }
                catch { /* Ignore malformed realm_access */ }
            }

            var identity = new ClaimsIdentity(claims, authenticationType: "Bearer");
            context.HttpContext.User = new ClaimsPrincipal(identity);
        }

        await next();
    }

}
