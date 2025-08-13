namespace Siem.Platform.User.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.User user, CancellationToken cancellationToken = default);

    #region UnitOfWork

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #endregion
}
