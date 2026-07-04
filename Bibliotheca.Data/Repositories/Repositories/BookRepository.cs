using Bibliotheca.Data.Context;
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;
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

    public async Task<PagedResult<Book>> SearchPagedAsync(
        BookSearchFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking().Where(b => b.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(b => b.Name.Contains(filter.Name));
        if (!string.IsNullOrWhiteSpace(filter.AuthorName))
            query = query.Where(b => b.Author.Contains(filter.AuthorName));
        if (filter.Language.HasValue)
            query = query.Where(b => b.LanguageEnum == filter.Language.Value);
        if (!string.IsNullOrWhiteSpace(filter.Publisher))
            query = query.Where(b => b.Publisher.Contains(filter.Publisher));
        if (filter.Condition.HasValue)
            query = query.Where(b => b.ConditionEnum == filter.Condition.Value);
        if (filter.YearFrom.HasValue)
            query = query.Where(b => b.PublicationYear >= filter.YearFrom.Value);
        if (filter.YearTo.HasValue)
            query = query.Where(b => b.PublicationYear <= filter.YearTo.Value);

        query = query.OrderBy(b => b.Name);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<Book> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
    }

    public async Task<PagedResult<Book>> GetByUserIdPagedAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking()
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
}