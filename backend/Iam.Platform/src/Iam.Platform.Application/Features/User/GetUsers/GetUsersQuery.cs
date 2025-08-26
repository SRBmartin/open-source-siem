using EBus.Abstractions;
using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.User.GetUsers;

public sealed record GetUsersQuery(
    int First = 0,
    int Max = 50,
    bool IncludeRoles = false,
    string? Search = null
) : IRequest<ApiResponse<List<UserListItemDto>>>;