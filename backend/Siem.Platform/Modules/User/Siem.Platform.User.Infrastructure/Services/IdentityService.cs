using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Application.DTOs.Identity.User.Create;
using Siem.Platform.User.Application.DTOs.User;
using System.Net.Http.Json;
using System.Text.Json;

namespace Siem.Platform.User.Infrastructure.Services;

public class IdentityService(
    HttpClient httpClient
) : IIdentityService
{
    private static readonly JsonSerializerOptions Json = new () { PropertyNameCaseInsensitive = true };
    private sealed class IamApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
    }

    public async Task<Result<CreateUserResponseDto>> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var resp = await httpClient.PostAsJsonAsync("api/users", request, Json, cancellationToken);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync(cancellationToken);
            return Result<CreateUserResponseDto>.Failure(
                new Error("iam.http_error", $"IAM returned {(int)resp.StatusCode}. {body}"));
        }

        var api = await resp.Content.ReadFromJsonAsync<IamApiResponse<CreateUserResponseDto>>(Json, cancellationToken);
        if (api is null)
            return Result<CreateUserResponseDto>.Failure(new Error("iam.deserialize", "Empty IAM response."));

        if (!api.Success)
            return Result<CreateUserResponseDto>.Failure(new Error("iam.failed", api.Message ?? "IAM returned failure."));

        if (api.Data is null)
            return Result<CreateUserResponseDto>.Failure(new Error("iam.no_data", "IAM returned no data."));

        return Result<CreateUserResponseDto>.Success(new CreateUserResponseDto(api.Data.ExternalId));
    }
}
