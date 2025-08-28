namespace Siem.Platform.Logging.Application.DTOs.Tags;

public sealed record IngestInstructionsDto(
    string OtlpHttpUrl,
    string HeaderName,
    string HeaderValue,
    string AuthTokenUrl,
    string ClientId,
    string Audience);