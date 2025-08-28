using Microsoft.EntityFrameworkCore;
using Siem.Platform.Logging.Domain.Entities;
using Siem.Platform.Logging.Infrastructure.Persistence.Configurations;

namespace Siem.Platform.Logging.Infrastructure.Persistence.Contexts;

public class LoggingDbContext : DbContext
{
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

    public DbSet<LogTag> LogTags => Set<LogTag>();
    public DbSet<LogTagAccess> LogTagAccesses => Set<LogTagAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Logging");
        modelBuilder.ApplyConfiguration(new LogTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LogTagAccessEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }

}
