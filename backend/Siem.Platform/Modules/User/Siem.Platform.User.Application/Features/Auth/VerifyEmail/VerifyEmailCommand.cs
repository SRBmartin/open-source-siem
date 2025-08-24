using EBus.Abstractions;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.User.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand (
    string UserId,
    string ActivationToken
) : IRequest<Result>;