using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Siem.Platform.Shared.Application.Abstractions.Services;
using Siem.Platform.Shared.Infrastructure.Configuration;
using System.Net.Http.Headers;
using System.Runtime;

namespace Siem.Platform.Shared.Infrastructure.Services.Security;

public class AuthVerificationService (
    HttpClient httpClient,
    IOptions<IamPlatformSettings> options
) : IAuthVerificationService
{
    private readonly IamPlatformSettings _settings = options.Value;

    public async Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.VerifyPath}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}
