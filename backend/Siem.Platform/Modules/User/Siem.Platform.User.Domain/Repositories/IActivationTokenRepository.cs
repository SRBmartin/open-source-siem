using Siem.Platform.User.Domain.Entities;

namespace Siem.Platform.User.Domain.Repositories;

public interface IActivationTokenRepository
{
    Task<ActivationToken?> GetActiveByUserAndHashAsync(Guid userId, string hashedToken, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
