using EBus.Abstractions;
using Iam.Platform.Application.DTOs.Auth;
using Iam.Platform.Application.Interfaces;
using Iam.Platform.Application.Models;

namespace Iam.Platform.Application.Features.Auth.Login;

public sealed class LoginCommandHandler (
    IKeycloakTokenService keycloakTokenService
) : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
{
    public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Username) || string.IsNullOrWhiteSpace(command.Password))
        {
            return ApiResponse<LoginResponseDto>.Fail("Username and password must be provided.");
        }

        try
        {
            var accessToken = await keycloakTokenService.GetPasswordTokenAsync(command.Username, command.Password, cancellationToken);
            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto { AccessToken = accessToken });
        }
        catch (HttpRequestException)
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid username or password.");
        }
        catch (Exception)
        {
            return ApiResponse<LoginResponseDto>.Fail("Login failed due to an unexpected error.");
        }

    }

}
