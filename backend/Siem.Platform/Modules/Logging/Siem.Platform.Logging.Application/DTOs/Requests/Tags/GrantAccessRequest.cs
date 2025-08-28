using Siem.Platform.Logging.Domain.Entities;

namespace Siem.Platform.Logging.Application.DTOs.Requests.Tags;

public sealed record GrantAccessRequest(
    Guid UserId,
    LogAccessRole Role
);