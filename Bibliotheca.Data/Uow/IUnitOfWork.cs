using Bibliotheca.Data.Repositories;

namespace Bibliotheca.Data.Uow;

/// <summary>
/// Agrupa os repositórios que compartilham o mesmo DbContext, permitindo salvar
/// (ou desfazer) as mudanças de vários repositórios em uma única transação.
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IAuthorRepository Authors { get; }
    IBookRepository Books { get; }
    ICommentRepository Comments { get; }
    IProfileRepository Profiles { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}