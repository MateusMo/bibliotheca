using System.Linq.Expressions;
using Bibliotheca.Data.Repositories.Dto;

namespace Bibliotheca.Data.Repositories;

/// <summary>
/// Contrato genérico de repositório. Qualquer repositório específico (ex.: IBookRepository)
/// deve herdar desta interface para reaproveitar o CRUD básico e adicionar métodos próprios.
/// </summary>
public interface IGenericRepository<TEntity> where TEntity : class
{
    // ---- Consultas básicas ----
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    // ---- Busca por predicado ----
    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    // ---- Busca paginada ----
    Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    // ---- Verificações ----
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    // ---- Escrita (CRUD) ----
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);

    // ---- Persistência ----
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}