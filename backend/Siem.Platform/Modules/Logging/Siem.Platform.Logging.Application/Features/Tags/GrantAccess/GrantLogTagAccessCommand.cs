using EBus.Abstractions;
using Siem.Platform.Logging.Domain.Entities;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Tags.GrantAccess;

public sealed record GrantLogTagAccessCommand(
    Guid TagId,
    Guid UserId,
    LogAccessRole Role
) : IRequest<Result>;
