using EBus.Abstractions;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.CreateUser;

public record CreateUserCommand (CreateUserDto User) : IRequest<ApiResponse<CreateUserResponseDto>>;
