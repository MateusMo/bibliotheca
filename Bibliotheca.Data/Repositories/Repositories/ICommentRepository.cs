// Bibliotheca.Data/Repositories/Repositories/ICommentRepository.cs
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Comment>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);

    Task<PagedResult<Comment>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<Comment>> GetByBookIdPagedAsync(Guid bookId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}