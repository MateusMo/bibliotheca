using Bibliotheca.Data.Context;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(BibliothecaContext context) : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => FindAsync(u => u.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => ExistsAsync(u => u.Email == email, cancellationToken);
}