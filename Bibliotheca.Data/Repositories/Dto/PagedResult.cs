namespace Bibliotheca.Data.Repositories.Dto;

/// <summary>
/// Representa o resultado de uma consulta paginada.
/// </summary>
public class PagedResult<TEntity>
{
    public IReadOnlyList<TEntity> Items { get; init; } = Array.Empty<TEntity>();
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
 
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
