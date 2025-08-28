using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siem.Platform.Logging.Domain.Entities;

namespace Siem.Platform.Logging.Infrastructure.Persistence.Configurations;

public class LogTagEntityConfiguration : IEntityTypeConfiguration<LogTag>
{
    public void Configure(EntityTypeBuilder<LogTag> builder)
    {
        builder.ToTable("LogTags");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Topic)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Partitions)
            .IsRequired();

        builder.Property(x => x.RetentionMs)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Topic).IsUnique();
    }
}
