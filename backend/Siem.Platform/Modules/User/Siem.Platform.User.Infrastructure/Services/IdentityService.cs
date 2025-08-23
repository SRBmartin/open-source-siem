using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using Siem.Platform.User.Application.Contracts;
using Siem.Platform.User.Application.DTOs.Identity.User.Create;
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

    public async Task<Result<bool>> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/users/exists?email={Uri.EscapeDataString(email)}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Result<bool>.Failure(new Error("iam.failed", $"Checking existence of user with email {email} failed with status code {(int)response.StatusCode}."));
        }

        var api = await response.Content.ReadFromJsonAsync<IamApiResponse<bool>>(Json, cancellationToken);
        if (api is null || !api.Success)
        {
            return Result<bool>.Failure(new Error("iam.deserialize", "Empty or unsuccessful IAM response while checking user existence."));
        }

        return Result<bool>.Success(api.Data);
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/users/{userId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure(new Error("iam.failed", $"Deleting of user with ID {userId} failed with status code {(int)response.StatusCode}."));
        }

        return Result.Success();
    }

}
