// Bibliotheca.Data/Repositories/Repositories/ICommentRepository.cs
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Comment>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
}