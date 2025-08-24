using Microsoft.AspNetCore.Mvc;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Api.Http;

public static class ResultToHttpExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase ctrl, Result<T> result)
    {
        if (result.IsSuccess)
            return ctrl.Ok(new ApiResponse<T>
            {
                Success = true,
                Data = result.Value,
                TraceId = ctrl.HttpContext.TraceIdentifier
            });

        var errors = result.Errors.Select(e => new ApiError(e.Code, e.Message)).ToList();
        var status = MapStatusCode(errors);

        return ctrl.StatusCode(status, new ApiResponse<T>
        {
            Success = false,
            Errors = errors,
            TraceId = ctrl.HttpContext.TraceIdentifier
        });
    }

    public static IActionResult ToActionResult(this ControllerBase ctrl, Result result)
    {
        if (result.IsSuccess)
            return ctrl.Ok(new ApiResponse<object> { Success = true, TraceId = ctrl.HttpContext.TraceIdentifier });

        var errors = result.Errors.Select(e => new ApiError(e.Code, e.Message)).ToList();
        var status = MapStatusCode(errors);

        return ctrl.StatusCode(status, new ApiResponse<object>
        {
            Success = false,
            Errors = errors,
            TraceId = ctrl.HttpContext.TraceIdentifier
        });
    }

    private static int MapStatusCode(List<ApiError> errors)
    {
        bool Has(string p) => errors.Any(e => e.Code.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        bool HasExact(string c) => errors.Any(e => e.Code.Equals(c, StringComparison.OrdinalIgnoreCase));

        // Client errors
        if (Has("validation.")) return StatusCodes.Status400BadRequest;
        if (Has("unauthorized.")) return StatusCodes.Status401Unauthorized;
        if (Has("forbidden.")) return StatusCodes.Status403Forbidden;
        if (Has("not_found.")) return StatusCodes.Status404NotFound;
        if (Has("conflict.")) return StatusCodes.Status409Conflict;
        if (Has("rate_limit.")) return StatusCodes.Status429TooManyRequests;

        // Activation tokens
        if (HasExact("activation.not_found_or_expired") || Has("activation.expired"))
            return StatusCodes.Status410Gone;
        if (Has("activation.")) return StatusCodes.Status400BadRequest;

        // Upstream/IAM issues
        if (Has("iam.http_error") || Has("iam.failed") || Has("iam.deserialize"))
            return StatusCodes.Status502BadGateway;

        // Server/infra
        if (Has("service_unavailable.")) return StatusCodes.Status503ServiceUnavailable;
        if (Has("timeout.")) return StatusCodes.Status504GatewayTimeout;
        if (Has("internal.") || HasExact("user.create_failed") || HasExact("user.email_send_failed"))
            return StatusCodes.Status500InternalServerError;

        return StatusCodes.Status400BadRequest;
    }
}
