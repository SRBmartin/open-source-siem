using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Application.Features.Auth.LogoutCommand;

public sealed record LogoutCommand (string UserId) : IRequest<Result>;
