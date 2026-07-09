using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Feed;
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;
using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Services;

public class FeedService : BaseService, IFeedService
{
    // Quantos itens buscar de cada fonte (livros/bibliotecas) para poder
    // mesclar e reordenar em memória antes de paginar. Trade-off ciente:
    // evita uma UNION real entre duas tabelas com formatos diferentes, ao
    // custo de trazer mais linhas do banco do que o necessário para a
    // página pedida. O teto de 300 protege contra páginas muito altas.
    // Se o catálogo crescer muito, o próximo passo é uma tabela de
    // projeção (feed_items) mantida por job/trigger.
    private const int MergeWindowMultiplier = 3;
    private const int MergeWindowCap = 300;

    private readonly IUnitOfWork _unitOfWork;

    public FeedService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<PagedResultDto<FeedItemDto>>> SearchAsync(BookFilterDto filter, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var hasAdvancedFilters = filter.YearFrom.HasValue || filter.YearTo.HasValue
            || filter.AddedFrom.HasValue || filter.AddedTo.HasValue
            || filter.PagesFrom.HasValue || filter.PagesTo.HasValue
            || filter.Language.HasValue || filter.Condition.HasValue
            || filter.ValueFrom.HasValue || filter.ValueTo.HasValue
            || filter.SortBy == BookSortOptionEnum.MostViewed;

        var bookSearchFilter = new BookSearchFilter
        {
            SearchText = filter.SearchText,
            YearFrom = filter.YearFrom,
            YearTo = filter.YearTo,
            AddedFrom = filter.AddedFrom,
            AddedTo = filter.AddedTo,
            PagesFrom = filter.PagesFrom,
            PagesTo = filter.PagesTo,
            Language = filter.Language,
            Condition = filter.Condition,
            ValueFrom = filter.ValueFrom,
            ValueTo = filter.ValueTo,
            SortBy = filter.SortBy
        };

        // Filtros avançados só existem para livros (ano, páginas, idioma etc.);
        // bibliotecas não têm esses atributos, então nesse caso o feed exibe
        // somente livros, com paginação normal no banco.
        if (hasAdvancedFilters)
        {
            var pagedBooks = await _unitOfWork.Books.SearchPagedAsync(bookSearchFilter, pageNumber, pageSize);

            var onlyBooks = new PagedResultDto<FeedItemDto>
            {
                Items = pagedBooks.Items.Select(ToBookFeedItem).ToList(),
                PageNumber = pagedBooks.PageNumber,
                PageSize = pagedBooks.PageSize,
                TotalCount = pagedBooks.TotalCount
            };

            return Success(onlyBooks);
        }

        var windowSize = Math.Min(pageNumber * pageSize * MergeWindowMultiplier, MergeWindowCap);

        var booksWindow = await _unitOfWork.Books.SearchPagedAsync(bookSearchFilter, 1, windowSize);
        var librariesWindow = await _unitOfWork.Libraries.SearchPagedAsync(filter.SearchText, 1, windowSize);

        var merged = booksWindow.Items.Select(ToBookFeedItem)
            .Concat(librariesWindow.Items.Select(ToLibraryFeedItem))
            .OrderByDescending(i => i.ProfileScore)
            .ThenByDescending(i => i.CreatedAt)
            .ToList();

        var page = merged
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dto = new PagedResultDto<FeedItemDto>
        {
            Items = page,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = booksWindow.TotalCount + librariesWindow.TotalCount
        };

        return Success(dto);
    }

    private static FeedItemDto ToBookFeedItem(Book book) => new()
    {
        Type = FeedItemType.Book,
        Id = book.Id,
        Title = book.Name,
        Subtitle = book.Author,
        CreatedAt = book.CreatedAt,
        UserName = book.User?.Name ?? string.Empty,
        ProfileScore = book.User?.Profile?.ProfileScore?.TotalScore ?? 0,
        PublicationYear = book.PublicationYear,
        PhotoCount = book.Photos.Length,
        LanguageEnum = book.LanguageEnum,
        ConditionEnum = book.ConditionEnum,
        ViewCount = book.ViewCount
    };

    private static FeedItemDto ToLibraryFeedItem(Library library) => new()
    {
        Type = FeedItemType.Library,
        Id = library.Id,
        Title = library.Title,
        CreatedAt = library.CreatedAt,
        UserName = library.User?.Name ?? string.Empty,
        ProfileScore = library.User?.Profile?.ProfileScore?.TotalScore ?? 0,
        BookCount = library.Books.Count
    };
}