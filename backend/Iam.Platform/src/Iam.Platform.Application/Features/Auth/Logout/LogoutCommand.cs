using EBus.Abstractions;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.Auth.Logout;

public sealed record LogoutCommand (string UserId) : IRequest<ApiResponse<bool>>;
