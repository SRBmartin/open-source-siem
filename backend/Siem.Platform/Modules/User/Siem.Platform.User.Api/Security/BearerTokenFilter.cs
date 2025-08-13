using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Siem.Platform.Shared.Application.Abstractions.Services;

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

        await next();
    }

}
