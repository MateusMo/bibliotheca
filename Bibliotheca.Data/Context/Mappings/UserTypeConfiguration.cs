using Bibliotheca.Domain.Domains;
using Bibliotheca.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("char(36)")
            .ValueGeneratedNever();

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(u => u.PlanType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PlanTypeEnum.Free);

        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.Email).IsUnique();
    }
}