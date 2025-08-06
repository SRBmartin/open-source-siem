using MailService.Application.DTOs;
using MailService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController (
    IEmailVerificationService verificationService    
) : ControllerBase
{
    [HttpPost("verify-email")]
    public async Task<IActionResult> SendVerification([FromBody] EmailVerifyRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await verificationService.SendVerificationEmailAsync(request.UserId, request.Email, request.Token, cancellationToken);

            return Accepted();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to send email to address {request.Email}.");
        }
    }

}
