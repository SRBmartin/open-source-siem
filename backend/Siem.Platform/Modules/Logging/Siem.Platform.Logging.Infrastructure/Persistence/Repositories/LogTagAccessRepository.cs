using Microsoft.EntityFrameworkCore;
using Siem.Platform.Logging.Domain.Entities;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Logging.Infrastructure.Persistence.Contexts;

namespace Siem.Platform.Logging.Infrastructure.Persistence.Repositories;

public class LogTagAccessRepository (
    LoggingDbContext dbContext    
) : ILogTagAccessRepository
{
    public Task AddAsync(LogTagAccess access, CancellationToken cancellationToken = default)
    {
        return dbContext.AddAsync(access, cancellationToken)
            .AsTask();
    }
    public async Task<LogTagAccess?> GetAsync(Guid tagId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.LogTagAccesses
            .FirstOrDefaultAsync(x => x.TagId == tagId && x.UserId == userId && !x.IsDeleted, cancellationToken);
    }
    public async Task<IReadOnlyList<LogTagAccess>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.LogTagAccesses
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await dbContext.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LogTagAccess>> GetForTagAsync(Guid tagId, CancellationToken cancellationToken = default)
        => await dbContext.LogTagAccesses.AsNoTracking().Where(x => x.TagId == tagId && !x.IsDeleted).ToListAsync(cancellationToken);

}
