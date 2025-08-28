using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Siem.Platform.Logging.Infrastructure.Persistence.Contexts;

public sealed class LoggingDbContextFactory : IDesignTimeDbContextFactory<LoggingDbContext>
{
    public LoggingDbContext CreateDbContext(string[] args)
    {
        // If you run EF from your host, platform-postgres is published on 5434.
        // Override with env var PLATFORM_DB when needed.
        var cs = Environment.GetEnvironmentVariable("PLATFORM_DB")
                 ?? "Host=localhost;Port=5434;Database=platform;Username=platform;Password=platform_pwd";

        var options = new DbContextOptionsBuilder<LoggingDbContext>()
            .UseNpgsql(cs, npgsql =>
            {
                //Keep migrations in the Logging schema
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "Logging");
            })
            .Options;

        return new LoggingDbContext(options);
    }
}
