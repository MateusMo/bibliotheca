using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface IAuthorRepository : IGenericRepository<Author>
{
    Task<IEnumerable<Author>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
}