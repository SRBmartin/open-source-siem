using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Application.Features.Auth.LoginCommand;

public sealed record LoginCommand 
(
    string Username,
    string Password
) : IRequest<Result<string>>;
