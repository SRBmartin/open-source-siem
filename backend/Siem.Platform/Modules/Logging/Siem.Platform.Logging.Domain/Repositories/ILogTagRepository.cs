using Siem.Platform.Logging.Domain.Entities;

namespace Siem.Platform.Logging.Domain.Repositories;

public interface ILogTagRepository
{
    Task AddAsync(LogTag tag, CancellationToken cancellationToken = default);
    Task<LogTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LogTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LogTag>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LogTag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    void Delete(LogTag tag, bool soft = true);

    #region Unit of Work

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    #endregion
}
