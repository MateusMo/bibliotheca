using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface IProfileScoreRepository : IGenericRepository<ProfileScore>
{
    Task<ProfileScore?> GetByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default);
}