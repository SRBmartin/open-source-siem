namespace Siem.Platform.Logging.Application.DTOs.Requests.Tags;

public sealed record TagAccessDto(
    Guid UserId,
    string Role
);