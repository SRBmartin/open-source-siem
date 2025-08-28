using Microsoft.Extensions.Caching.Memory;
using Siem.Platform.Shared.Application.Abstractions.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace Siem.Platform.Shared.Infrastructure.Http;

public sealed class ClientAccessTokenHandler (
    IKeycloakTokenService tokenService,
    IMemoryCache cache
) : DelegatingHandler
{
    private const string CacheKey = "Keycloak_ClientAccessToken";
    private static readonly SemaphoreSlim _tokenLock = new(1, 1);
    private static readonly TimeSpan SafetyBuffer = TimeSpan.FromSeconds(30);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await GetTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(CacheKey, out string? token) && !string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            if (cache.TryGetValue(CacheKey, out token) && !string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            token = await tokenService.GetClientCredentialsTokenAsync(cancellationToken);

            var ttl = GetTtlFromJwt(token, SafetyBuffer);

            if (ttl <= TimeSpan.Zero)
            {
                ttl = TimeSpan.FromMinutes(5);
            }

            cache.Set(CacheKey, token!, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });

            return token;
        }
        finally
        {
            _tokenLock.Release();
        }

    }

    private static TimeSpan GetTtlFromJwt(string jwt, TimeSpan buffer)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(jwt)) return TimeSpan.Zero;

            var token = handler.ReadJwtToken(jwt);
            long? expUnix = token.Payload.Expiration;
            DateTimeOffset exp;

            if (expUnix.HasValue)
            {
                exp = DateTimeOffset.FromUnixTimeSeconds(expUnix.Value);
            }
            else
            {
                exp = token.ValidTo == DateTime.MinValue
                    ? DateTimeOffset.MinValue
                    : new DateTimeOffset(token.ValidTo, TimeSpan.Zero);
            }

            var now = DateTimeOffset.UtcNow;
            var ttl = exp - now - buffer;

            return ttl > TimeSpan.Zero ? ttl : TimeSpan.Zero;
        }
        catch
        {
            return TimeSpan.Zero;
        }
    }

}