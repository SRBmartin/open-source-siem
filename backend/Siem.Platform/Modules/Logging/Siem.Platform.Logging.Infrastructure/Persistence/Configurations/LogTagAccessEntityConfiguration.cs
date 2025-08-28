using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siem.Platform.Logging.Domain.Entities;

namespace Siem.Platform.Logging.Infrastructure.Persistence.Configurations;

class LogTagAccessEntityConfiguration : IEntityTypeConfiguration<LogTagAccess>
{
    public void Configure(EntityTypeBuilder<LogTagAccess> b)
    {
        b.ToTable("LogTagAccess");
        b.HasKey(x => x.Id);

        b.Property(x => x.TagId).IsRequired();
        b.Property(x => x.UserId).IsRequired();
        b.Property(x => x.Role).HasConversion<int>().IsRequired();
        b.Property(x => x.IsDeleted).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasIndex(x => new { x.TagId, x.UserId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }

}
