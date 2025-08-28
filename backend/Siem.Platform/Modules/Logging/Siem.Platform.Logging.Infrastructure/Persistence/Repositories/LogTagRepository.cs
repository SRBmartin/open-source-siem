using Microsoft.EntityFrameworkCore;
using Siem.Platform.Logging.Domain.Entities;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Logging.Infrastructure.Persistence.Contexts;

namespace Siem.Platform.Logging.Infrastructure.Persistence.Repositories;

public class LogTagRepository (
    LoggingDbContext dbContext    
) : ILogTagRepository
{
    public async Task AddAsync(LogTag tag, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(tag, cancellationToken);
    }

    public async Task<LogTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.LogTags.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<LogTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var n = name.Trim().ToLowerInvariant();
        return await dbContext.LogTags.FirstOrDefaultAsync(t => t.Name == n && !t.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<LogTag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.LogTags.AsNoTracking().Where(t => !t.IsDeleted).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var n = name.Trim().ToLowerInvariant();
        return dbContext.LogTags.AnyAsync(t => t.Name == n && !t.IsDeleted, cancellationToken);
    }

    public void Delete(LogTag tag, bool soft = true)
    {
        if (soft)
        {
            tag.MarkDeleted();
            dbContext.Update(tag);
        }
        else
        {
            dbContext.Remove(tag);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LogTag>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var set = ids.ToHashSet();
        return await dbContext.LogTags
            .AsNoTracking()
            .Where(t => set.Contains(t.Id) && !t.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
