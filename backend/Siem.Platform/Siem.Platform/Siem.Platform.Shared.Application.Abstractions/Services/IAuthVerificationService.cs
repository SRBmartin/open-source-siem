namespace Siem.Platform.Shared.Application.Abstractions.Services;

public interface IAuthVerificationService
{
    Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);
}
