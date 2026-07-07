using Bibliotheca.Data.Context;
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;
using Bibliotheca.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Data.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(BibliothecaContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await DbSet
            .Where(b => b.Id == id)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(b => b.ViewCount, b => b.ViewCount + 1),
                cancellationToken);
    }

    // Include de User -> Profile -> ProfileScore: uma única query com JOINs,
    // necessária pra exibir nome do dono e pontuação do perfil na listagem
    // sem cair em N+1 (uma consulta de perfil por livro).
    private IQueryable<Book> QueryWithOwnerInfo()
        => DbSet.AsNoTracking()
            .Include(b => b.User)
                .ThenInclude(u => u.Profile)
                    .ThenInclude(p => p!.ProfileScore);

    public async Task<PagedResult<Book>> SearchPagedAsync(
        BookSearchFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = QueryWithOwnerInfo().Where(b => b.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var text = filter.SearchText.Trim();
            query = query.Where(b =>
                b.Name.Contains(text) ||
                b.Author.Contains(text) ||
                b.ISBN.Contains(text));
        }

        if (filter.YearFrom.HasValue)
            query = query.Where(b => b.PublicationYear >= filter.YearFrom.Value);
        if (filter.YearTo.HasValue)
            query = query.Where(b => b.PublicationYear <= filter.YearTo.Value);

        if (filter.AddedFrom.HasValue)
        {
            var from = new DateTimeOffset(DateTime.SpecifyKind(filter.AddedFrom.Value.Date, DateTimeKind.Utc));
            query = query.Where(b => b.CreatedAt >= from);
        }

        if (filter.AddedTo.HasValue)
        {
            var to = new DateTimeOffset(DateTime.SpecifyKind(filter.AddedTo.Value.Date.AddDays(1), DateTimeKind.Utc));
            query = query.Where(b => b.CreatedAt < to);
        }

        if (filter.PagesFrom.HasValue)
            query = query.Where(b => b.Pages >= filter.PagesFrom.Value);
        if (filter.PagesTo.HasValue)
            query = query.Where(b => b.Pages <= filter.PagesTo.Value);

        if (filter.Language.HasValue)
            query = query.Where(b => b.LanguageEnum == filter.Language.Value);

        if (filter.Condition.HasValue)
            query = query.Where(b => b.ConditionEnum == filter.Condition.Value);

        if (filter.ValueFrom.HasValue)
            query = query.Where(b => b.EstimatedValue >= filter.ValueFrom.Value);
        if (filter.ValueTo.HasValue)
            query = query.Where(b => b.EstimatedValue <= filter.ValueTo.Value);

        query = filter.SortBy switch
        {
            BookSortOptionEnum.MostViewed => query.OrderByDescending(b => b.ViewCount),
            _ => query.OrderByDescending(b => b.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<Book> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
    }

    public async Task<PagedResult<Book>> GetByUserIdPagedAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = QueryWithOwnerInfo()
            .Where(b => b.UserId == userId && b.IsActive)
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Book>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BookOwnershipStats> GetOwnershipStatsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var stats = await DbSet.AsNoTracking()
            .Where(b => b.UserId == userId && b.IsActive && b.IsOwner)
            .GroupBy(b => 1)
            .Select(g => new BookOwnershipStats
            {
                OwnedBooksCount = g.Count(),
                AveragePublicationYear = (int)g.Average(b => b.PublicationYear),
                TotalViews = g.Sum(b => b.ViewCount)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new BookOwnershipStats();
    }
}