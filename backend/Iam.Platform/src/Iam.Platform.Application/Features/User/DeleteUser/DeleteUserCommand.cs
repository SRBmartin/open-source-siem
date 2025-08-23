using EBus.Abstractions;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.DeleteUser;

public sealed record DeleteUserCommand (string UserId): IRequest<ApiResponse<bool>>;
