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

    public Task<Book?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<IEnumerable<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking().Include(b => b.Authors).ToListAsync()
            .ContinueWith(t => (IEnumerable<Book>)t.Result, cancellationToken);

    public async Task<IEnumerable<Book>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Include(b => b.Authors)
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Book>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Include(b => b.Authors)
            .Where(b => b.Authors.Any(a => a.Id == authorId))
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Book>> SearchAsync(BookSearchFilter filter, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(b => b.Authors)
            .Where(b => b.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(b => b.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.AuthorName))
            query = query.Where(b => b.Authors.Any(a => a.Name.Contains(filter.AuthorName)));

        if (!string.IsNullOrWhiteSpace(filter.Language))
            query = query.Where(b => b.Language == filter.Language);

        if (!string.IsNullOrWhiteSpace(filter.Publisher))
            query = query.Where(b => b.Publisher.Contains(filter.Publisher));

        if (filter.Condition.HasValue)
            query = query.Where(b => b.Condition == filter.Condition.Value);

        if (filter.YearFrom.HasValue)
            query = query.Where(b => b.PublicationYear >= filter.YearFrom.Value);

        if (filter.YearTo.HasValue)
            query = query.Where(b => b.PublicationYear <= filter.YearTo.Value);

        return await query
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);
    }

    // NOVO: paginação da listagem "meus livros" no Perfil.
    // Já filtra por IsActive aqui (soft delete não deve aparecer na lista).
    public async Task<PagedResult<Book>> GetByUserIdPagedAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking()
            .Include(b => b.Authors)
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