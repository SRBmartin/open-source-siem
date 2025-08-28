using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siem.Platform.User.Domain.Entities;

namespace Siem.Platform.User.Infrastructure.Persistence.Configurations;

public class ActivationTokenEntityConfiguration : IEntityTypeConfiguration<ActivationToken>
{
    public void Configure(EntityTypeBuilder<ActivationToken> b)
    {
        b.ToTable("ActivationTokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.Token).IsRequired().HasMaxLength(512);
        b.Property(x => x.ExpiresAt).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.IsUsed).IsRequired();

        b.HasIndex(x => x.Token).IsUnique();

        b.HasOne(x => x.User)
         .WithMany(u => u.ActivationTokens)
         .HasForeignKey(x => x.UserId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);
    }
}
