using System.Text.Json;
using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class BookTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnType("char(36)")
            .ValueGeneratedNever();

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(b => b.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(b => b.UserId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(b => b.IsOwner)
            .IsRequired();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(300);

        // Autor agora é texto livre digitado pelo próprio usuário no cadastro do livro.
        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(b => b.Description)
            .HasColumnType("text");

        builder.Property(b => b.PublicationYear)
            .IsRequired();

        // Mantém o nome de coluna "Language" pra não gerar rename desnecessário na migration.
        builder.Property(b => b.LanguageEnum)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnName("Language");

        builder.Property(b => b.Publisher)
            .HasMaxLength(200);

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.Pages)
            .IsRequired();

        builder.Property(b => b.EstimatedValue)
            .IsRequired()
            .HasColumnType("decimal(12,2)");

        builder.Property(b => b.ConditionEnum)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Photos)
            .HasConversion(
                photos => JsonSerializer.Serialize(photos, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<string[]>(json, (JsonSerializerOptions?)null) ?? Array.Empty<string>())
            .HasColumnType("json");

        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => b.Name);
        builder.HasIndex(b => b.Author);
        builder.HasIndex(b => b.ISBN).IsUnique();
        builder.HasIndex(b => b.UserId);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sem mais HasMany/BookAuthors — a tabela de junção deixa de existir.
    }
}