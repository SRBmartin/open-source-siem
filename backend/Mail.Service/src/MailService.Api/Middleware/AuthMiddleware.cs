using MailService.Application.Interfaces;

namespace MailService.Api.Middleware;

public class AuthMiddleware (
    RequestDelegate next,
    ILogger<AuthMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext ctx, IAuthVerificationService verifier)
    {
        var header = ctx.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(header) ||
            !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Authorization header is missing or invalid.");
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var token = header.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            logger.LogWarning("Token is missing in the Authorization header.");
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var isActive = await verifier.VerifyTokenAsync(token);
        if (!isActive)
        {
            logger.LogWarning("Token verification failed for token: {Token}", token);
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(ctx);
    }

}
