using Siem.Platform.Logging.Domain.Entities;

namespace Siem.Platform.Logging.Domain.Repositories;

public interface ILogTagAccessRepository
{
    Task AddAsync(LogTagAccess access, CancellationToken cancellationToken = default);
    Task<LogTagAccess?> GetAsync(Guid tagId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LogTagAccess>> GetForTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LogTagAccess>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
