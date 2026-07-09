using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Data.Context;

public class BibliothecaContext : DbContext
{
    public BibliothecaContext(DbContextOptions<BibliothecaContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ProfileScore> ProfileScores => Set<ProfileScore>();
    public DbSet<Library> Libraries => Set<Library>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BibliothecaContext).Assembly);
    }
}