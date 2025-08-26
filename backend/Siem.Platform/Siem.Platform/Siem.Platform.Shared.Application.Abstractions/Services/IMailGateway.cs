using Siem.Platform.Shared.Application.Abstractions.Common.DTOs;

namespace Siem.Platform.Shared.Application.Abstractions.Services;

public interface IMailGateway
{
    Task<bool> SendVerificationEmailAsync(EmailVerifyRequestDto request, CancellationToken cancellationToken = default);
}
