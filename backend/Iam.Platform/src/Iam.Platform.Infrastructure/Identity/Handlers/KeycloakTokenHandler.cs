using Iam.Platform.Application.Interfaces;
using System.Net.Http.Headers;

namespace Iam.Platform.Infrastructure.Identity.Handlers;

public class KeycloakTokenHandler (
    IKeycloakTokenService tokenService    
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenService.GetClientCredentialsTokenAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

}
