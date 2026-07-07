using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bibliotheca.Data.Context.Mappings;

public class ProfileScoreTypeConfiguration : IEntityTypeConfiguration<ProfileScore>
{
    public void Configure(EntityTypeBuilder<ProfileScore> builder)
    {
        builder.ToTable("ProfileScores");

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

        builder.Property(p => p.ProfileId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(p => p.TotalBooksAverageYear).IsRequired();
        builder.Property(p => p.TotalViews).IsRequired();
        builder.Property(p => p.TotalYearsOnline).IsRequired();
        builder.Property(p => p.TotalScore).IsRequired();

        builder.HasIndex(p => p.ProfileId).IsUnique();

        // Índice para quando o Feed passar a ordenar perfis pela pontuação.
        builder.HasIndex(p => p.TotalScore);

        builder.HasOne(p => p.Profile)
            .WithOne(pr => pr.ProfileScore)
            .HasForeignKey<ProfileScore>(p => p.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}