using Microsoft.EntityFrameworkCore;
using Siem.Platform.User.Domain.Entities;
using Siem.Platform.User.Domain.Repositories;
using Siem.Platform.User.Infrastructure.Persistence.Contexts;

namespace Siem.Platform.User.Infrastructure.Persistence.Repositories;

public class ActivationTokenRepository(
    UserDbContext dbContext
) : IActivationTokenRepository
{
    public async Task<ActivationToken?> GetActiveByUserAndHashAsync(Guid userId, string hashedToken, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<ActivationToken>()
            .Where(t => t.UserId.Equals(userId) &&
                    t.Token == hashedToken && 
                    !t.IsUsed &&
                    t.ExpiresAt > DateTimeOffset.UtcNow)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
