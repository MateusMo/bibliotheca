using Bibliotheca.Data.Context;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public class ProfileScoreRepository : GenericRepository<ProfileScore>, IProfileScoreRepository
{
    public ProfileScoreRepository(BibliothecaContext context) : base(context)
    {
    }

    public Task<ProfileScore?> GetByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default)
        => FindAsync(p => p.ProfileId == profileId, cancellationToken);
}