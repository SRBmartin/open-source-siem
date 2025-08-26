using EBus.Abstractions;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ExistsUser;

public record ExistsUserCommand (string Email) : IRequest<ApiResponse<bool>>;
