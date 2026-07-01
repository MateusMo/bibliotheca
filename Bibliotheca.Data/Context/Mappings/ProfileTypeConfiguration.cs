using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class ProfileTypeConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnType("char(36)")
            .ValueGeneratedNever();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(p => p.Description)
            .HasColumnType("text");

        builder.Property(p => p.Contact)
            .HasMaxLength(200);

        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.UserId).IsUnique();

        builder.HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}