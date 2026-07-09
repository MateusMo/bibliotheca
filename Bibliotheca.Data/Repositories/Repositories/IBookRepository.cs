using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<IEnumerable<Book>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PagedResult<Book>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Book>> SearchPagedAsync(BookSearchFilter filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookOwnershipStats> GetOwnershipStatsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Rastreados (sem AsNoTracking): usados para anexar livros à coleção
    // Books de uma Library dentro do mesmo DbContext.
    Task<List<Book>> GetTrackedByIdsForUserAsync(Guid userId, IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}