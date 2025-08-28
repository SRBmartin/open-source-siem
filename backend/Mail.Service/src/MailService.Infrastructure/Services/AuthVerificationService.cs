using MailService.Application.Interfaces;
using MailService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace MailService.Infrastructure.Services;

public class AuthVerificationService (
    HttpClient httpClient,
    IOptions<IamPlatformSettings> options,
    ILogger<AuthVerificationService> logger
) : IAuthVerificationService
{
    private readonly IamPlatformSettings _settings = options.Value;

    /// <inheritdoc/>
    public async Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.VerifyPath);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request, cancellationToken);

        logger.LogInformation($"Status code of verification is {response.StatusCode.ToString()}");

        return response.IsSuccessStatusCode;
    }
}
