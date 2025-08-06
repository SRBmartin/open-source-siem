using Iam.Platform.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Iam.Platform.Api.Utilities;

public static class HttpResponseMapper
{
    public static IActionResult ToActionResult<T>(this ApiResponse<T> response, ControllerBase controller, string? locationRouteName = null)
    {
        if (response.Success)
        {
            if (response.Data is null)
                return controller.NoContent();

            if (!string.IsNullOrWhiteSpace(locationRouteName))
                return controller.CreatedAtAction(locationRouteName, new { id = response.Data }, response);

            return controller.Ok(response);
        }

        if (IsValidationError(response.Message))
            return controller.BadRequest(response);

        if (IsConflict(response.Message))
            return controller.Conflict(response);

        return controller.StatusCode(500, response);
    }

    private static bool IsValidationError(string? msg) =>
        msg is not null &&
        (msg.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
         msg.Contains("required", StringComparison.OrdinalIgnoreCase) ||
         msg.Contains("invalid", StringComparison.OrdinalIgnoreCase));

    private static bool IsConflict(string? msg) =>
        msg is not null && msg.Contains("already exists", StringComparison.OrdinalIgnoreCase);

}
