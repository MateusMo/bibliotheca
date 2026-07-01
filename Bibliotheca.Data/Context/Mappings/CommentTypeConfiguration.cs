using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class CommentTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnType("char(36)")
            .ValueGeneratedNever();

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(c => c.BookId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(c => c.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.Link)
            .HasMaxLength(500);

        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.BookId);

        builder.HasOne(c => c.Book)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}