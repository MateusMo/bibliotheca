using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class LibraryTypeConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("Libraries");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnType("char(36)")
            .ValueGeneratedNever();

        builder.Property(l => l.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(l => l.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(l => l.UserId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .HasColumnType("text");

        builder.HasIndex(l => l.IsActive);
        builder.HasIndex(l => l.UserId);

        builder.HasOne(l => l.User)
            .WithMany(u => u.Libraries)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many explícito só para nomear a tabela de junção;
        // FKs, PK composta (BookId, LibraryId) e cascade ficam na convenção padrão do EF.
        builder.HasMany(l => l.Books)
            .WithMany(b => b.Libraries)
            .UsingEntity(j => j.ToTable("BookLibraries"));
        
        builder.HasIndex(l => l.IsActive);
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.Title); 
    }
}