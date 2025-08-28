using EBus.Abstractions;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ChangePassword;

public sealed record ChangePasswordCommand
(
    string UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<ApiResponse<bool>>;
