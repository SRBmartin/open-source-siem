using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Siem.Platform.User.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireRealmRoleAttribute : TypeFilterAttribute
{
    public RequireRealmRoleAttribute(params string[] roles) : base(typeof(RequireRealmRoleFilter))
    {
        Arguments = new object[] { roles ?? Array.Empty<string>() };
    }

    private sealed class RequireRealmRoleFilter(string[] roles) : IAsyncActionFilter
    {
        private readonly HashSet<string> _required =
            roles.Select(r => r?.Trim().ToLowerInvariant())
                 .Where(r => !string.IsNullOrWhiteSpace(r))
                 .ToHashSet(StringComparer.OrdinalIgnoreCase)!;

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }

            var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value.ToLowerInvariant());
            if (!_required.Overlaps(userRoles))
            {
                context.Result = new ForbidResult();
                return Task.CompletedTask;
            }

            return next();
        }
    }
}
