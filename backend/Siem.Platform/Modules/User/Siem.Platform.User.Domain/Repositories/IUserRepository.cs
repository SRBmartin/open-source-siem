namespace Siem.Platform.User.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Entities.User?> GetUserByUserId(Guid userId, CancellationToken cancellationToken = default);
    Task<Entities.User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.User user, CancellationToken cancellationToken = default);
    void Delete(Entities.User user, bool soft = true);

    #region UnitOfWork

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #endregion
}
