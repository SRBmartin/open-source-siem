using EBus.Abstractions;
using Iam.Platform.Application.DTOs.Auth;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.Auth.Login;

public sealed record LoginCommand (
    string Username,
    string Password
) : IRequest<ApiResponse<LoginResponseDto>>;