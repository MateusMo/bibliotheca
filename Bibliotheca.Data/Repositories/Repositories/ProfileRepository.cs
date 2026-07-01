using Bibliotheca.Data.Context;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
{
    public ProfileRepository(BibliothecaContext context) : base(context)
    {
    }

    public Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => FindAsync(p => p.UserId == userId, cancellationToken);
}