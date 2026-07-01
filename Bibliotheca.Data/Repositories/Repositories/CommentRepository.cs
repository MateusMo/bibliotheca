using Bibliotheca.Data.Context;
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
}