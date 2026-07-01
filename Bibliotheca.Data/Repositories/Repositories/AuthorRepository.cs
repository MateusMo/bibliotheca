using Bibliotheca.Data.Context;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(BibliothecaContext context) : base(context)
    {
    }

    public Task<IEnumerable<Author>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
        => FindAllAsync(a => a.Name.Contains(name), cancellationToken);
}