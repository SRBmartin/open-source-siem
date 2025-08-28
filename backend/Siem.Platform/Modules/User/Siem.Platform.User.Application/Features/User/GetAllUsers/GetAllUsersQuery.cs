using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.DTOs.User;

namespace Siem.Platform.User.Application.Features.User.GetAllUsers;

public sealed record GetAllUsersQuery(
    int? First = null,
    int? Max = null,
    string? Search = null
) : IRequest<Result<List<UserDto>>>;
