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
        if (errors.Any(e => e.Code.StartsWith("validation.", StringComparison.OrdinalIgnoreCase))) return StatusCodes.Status400BadRequest;
        if (errors.Any(e => e.Code.StartsWith("conflict.", StringComparison.OrdinalIgnoreCase))) return StatusCodes.Status409Conflict;
        if (errors.Any(e => e.Code.StartsWith("not_found.", StringComparison.OrdinalIgnoreCase))) return StatusCodes.Status404NotFound;
        if (errors.Any(e => e.Code.StartsWith("unauthorized.", StringComparison.OrdinalIgnoreCase))) return StatusCodes.Status401Unauthorized;
        return StatusCodes.Status400BadRequest;
    }
}
