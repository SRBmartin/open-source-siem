using Siem.Platform.Shared.Application.Abstractions.Common.DTOs;
using Siem.Platform.Shared.Application.Abstractions.Services;
using System.Net.Http.Json;

namespace Siem.Platform.Shared.Infrastructure.Services;

public class MailGateway (
    HttpClient httpClient    
) : IMailGateway
{
    public async Task<bool> SendVerificationEmailAsync(EmailVerifyRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/email/verify-email", request, cancellationToken);

        if (response.IsSuccessStatusCode) return true;

        return false;
    }
}
