using Bibliotheca.Data.Context;
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Data.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(BibliothecaContext context) : base(context)
    {
    }

    public Task<IEnumerable<Comment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => FindAllAsync(c => c.UserId == userId, cancellationToken);

    public Task<IEnumerable<Comment>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.BookId == bookId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IEnumerable<Comment>)t.Result, cancellationToken);

    public async Task<PagedResult<Comment>> GetByUserIdPagedAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking()
            .Include(c => c.Book)
            .Where(c => c.UserId == userId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Comment>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<Comment>> GetByBookIdPagedAsync(
        Guid bookId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = DbSet.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.BookId == bookId && c.IsActive)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Comment>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}