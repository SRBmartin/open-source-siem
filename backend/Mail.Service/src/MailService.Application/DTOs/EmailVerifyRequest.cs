namespace MailService.Application.DTOs;

public class EmailVerifyRequest
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}
