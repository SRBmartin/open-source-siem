namespace Siem.Platform.User.Application.DTOs.Identity.User.Create;

public sealed record CreateUserRequestDto (
    string Email,
    string FirstName,
    string Lastname,
    string Password
);
