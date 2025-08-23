using Microsoft.EntityFrameworkCore;
using Siem.Platform.User.Domain.Repositories;
using Siem.Platform.User.Infrastructure.Persistence.Contexts;

namespace Siem.Platform.User.Infrastructure.Persistence.Repositories;

public class UserRepository (
    UserDbContext dbContext
) : IUserRepository
{
    public async Task AddAsync(Domain.Entities.User user, CancellationToken cancellationToken = default)
    {
        dbContext.Add(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AnyAsync(t => t.Email.Equals(email), cancellationToken);
    }

    public void Delete(Domain.Entities.User user, bool soft = true)
    {
        if (soft)
        {
            user.Delete();
            dbContext.Update(user);
        }
        else
        {
            dbContext.Remove(user);
        }

        }

    #region UnitOfWork

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
