namespace Siem.Platform.User.Application.DTOs.Auth;

public record VerifyEmailDto (
    string UserId,
    string ActivationToken
);
