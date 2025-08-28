using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Iam.Platform.Api.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequireClientAccessTokenAttribute : AuthorizeAttribute
{
    public const string PolicyName = "RequireAccessToken";

    public RequireClientAccessTokenAttribute(string? roles = null)
    {
        Policy = PolicyName;
        if (!string.IsNullOrWhiteSpace(roles))
            Roles = roles;

        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
    }
}
