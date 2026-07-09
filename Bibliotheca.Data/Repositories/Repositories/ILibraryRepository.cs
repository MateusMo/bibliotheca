using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface ILibraryRepository : IGenericRepository<Library>
{
    Task<Library?> GetByIdWithBooksAsync(Guid id, CancellationToken cancellationToken = default);

    // Versão rastreada (sem AsNoTracking): necessária para adicionar/remover
    // livros da coleção Books (many-to-many) antes de SaveChangesAsync.
    Task<Library?> GetByIdTrackedWithBooksAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Library>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Library>> SearchPagedAsync(string? searchText, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}