using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Application.Features.User.ChangePassword;

public sealed record ChangePasswordCommand 
(
    string UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result>;
