using Bibliotheca.Data.Context;
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Data.Repositories;

public class LibraryRepository : GenericRepository<Library>, ILibraryRepository
{
    public LibraryRepository(BibliothecaContext context) : base(context)
    {
    }

    public Task<Library?> GetByIdWithBooksAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking()
            .Include(l => l.Books)
            .Include(l => l.User)
            .ThenInclude(u => u.Profile)
            .ThenInclude(p => p!.ProfileScore)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<PagedResult<Library>> SearchPagedAsync(
        string? searchText, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking()
            .Include(l => l.Books)
            .Include(l => l.User)
            .ThenInclude(u => u.Profile)
            .ThenInclude(p => p!.ProfileScore)
            .Where(l => l.IsActive);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var text = searchText.Trim();
            query = query.Where(l => l.Title.Contains(text));
        }

        query = query
            .OrderByDescending(l => l.User.Profile != null && l.User.Profile.ProfileScore != null
                ? l.User.Profile.ProfileScore.TotalScore
                : 0)
            .ThenByDescending(l => l.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<Library> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
    }

    public Task<Library?> GetByIdTrackedWithBooksAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet
            .Include(l => l.Books)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<PagedResult<Library>> GetByUserIdPagedAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking()
            .Include(l => l.Books)
            .Where(l => l.UserId == userId && l.IsActive)
            .OrderByDescending(l => l.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Library>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}