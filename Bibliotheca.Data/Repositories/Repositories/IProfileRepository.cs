using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Data.Repositories;

public interface IProfileRepository : IGenericRepository<Profile>
{
    Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}