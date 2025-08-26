namespace Siem.Platform.User.Application.DTOs.Auth;

public sealed record LoginRequestDto
(
    string Username,
    string Password
);
