using EBus.Abstractions;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.ActivateEmail;

public record ActivateEmailCommand (string UserId) : IRequest<ApiResponse<bool>>;
