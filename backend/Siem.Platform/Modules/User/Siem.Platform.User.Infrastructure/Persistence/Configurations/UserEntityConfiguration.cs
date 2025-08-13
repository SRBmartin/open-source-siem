using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siem.Platform.User.Domain.Entities;

namespace Siem.Platform.User.Infrastructure.Persistence.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<Domain.Entities.User>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .IsRequired();

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(u => u.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.CreatedAt)
               .IsRequired();

        var nav = builder.Metadata.FindNavigation(nameof(Domain.Entities.User.ActivationTokens))!;
        nav.SetField("_activationTokens");
        nav.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
