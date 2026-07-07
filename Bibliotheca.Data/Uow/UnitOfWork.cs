using Bibliotheca.Data.Context;
using Bibliotheca.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bibliotheca.Data.Uow;

public class UnitOfWork : IUnitOfWork
{
    private readonly BibliothecaContext _context;
    private IDbContextTransaction? _transaction;

    private IBookRepository? _books;
    private ICommentRepository? _comments;
    private IProfileRepository? _profiles;
    private IUserRepository? _users;
    private IProfileScoreRepository? _profileScores;
    

    public UnitOfWork(BibliothecaContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public IProfileScoreRepository ProfileScores => _profileScores ??= new ProfileScoreRepository(_context);
    public IBookRepository Books => _books ??= new BookRepository(_context);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_context);
    public IProfileRepository Profiles => _profiles ??= new ProfileRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_transaction is not null)
                await _transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();

        await _context.DisposeAsync();
    }
}