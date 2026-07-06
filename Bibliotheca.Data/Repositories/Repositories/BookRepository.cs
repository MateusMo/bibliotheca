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
    
    public async Task<PagedResult<Book>> SearchPagedAsync(
        BookSearchFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking().Where(b => b.IsActive);

        // Busca livre: título, autor ou ISBN. Contains vira LIKE '%texto%' —
        // não usa índice B-Tree por causa do wildcard à esquerda, mas é o
        // trade-off aceitável para busca textual sem infra de full-text ainda.
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

        // Faixa de data de cadastro. Normaliza para o início/fim do dia em UTC
        // pra "Adicionado até 04/07/2026" incluir o dia inteiro, não só 00:00.
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

        // Feed mostra os mais recentes primeiro.
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