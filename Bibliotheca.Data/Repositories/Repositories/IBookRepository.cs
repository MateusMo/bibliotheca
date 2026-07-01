using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetByIdWithAuthorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> SearchAsync(BookSearchFilter filter, CancellationToken cancellationToken = default);
    Task<PagedResult<Book>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}