using Microsoft.EntityFrameworkCore;
using Siem.Platform.User.Domain.Entities;
using Siem.Platform.User.Infrastructure.Persistence.Configurations;

namespace Siem.Platform.User.Infrastructure.Persistence.Contexts;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.User> Users => Set<Domain.Entities.User>();
    public DbSet<ActivationToken> ActivationTokens => Set<ActivationToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("User");
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ActivationTokenEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
