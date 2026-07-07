using Bibliotheca.Data.Repositories;

namespace Bibliotheca.Data.Uow;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IBookRepository Books { get; }
    ICommentRepository Comments { get; }
    IProfileRepository Profiles { get; }
    IUserRepository Users { get; }
    IProfileScoreRepository ProfileScores { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}