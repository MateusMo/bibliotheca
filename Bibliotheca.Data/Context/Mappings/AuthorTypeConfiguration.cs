using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class AuthorTypeConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnType("char(36)")
            .ValueGeneratedNever();

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(a => a.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Photo)
            .HasMaxLength(500);

        builder.Property(a => a.Description)
            .HasColumnType("text");

        builder.Property(a => a.BirthDay)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(a => a.DeathDay)
            .HasColumnType("date");

        builder.HasIndex(a => a.IsActive);
        builder.HasIndex(a => a.Name);
    }
}