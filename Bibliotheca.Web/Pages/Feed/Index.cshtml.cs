using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Services;
using Bibliotheca.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Feed;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private const int PageSize = 20;

    // Quantas páginas mostrar ao redor da atual antes de recorrer a "...".
    // Ex.: 1 ... 6 7 [8] 9 10 ... 20
    private const int Window = 2;

    private readonly IBookService _bookService;
    
    public List<SelectListItem> SortOptions { get; } =
        Enum.GetValues<BookSortOptionEnum>()
            .Select(s => new SelectListItem(
                s == BookSortOptionEnum.MostViewed ? "Mais vistos" : "Adicionados recentemente",
                s.ToString()))
            .ToList();
    
    public IndexModel(IBookService bookService) => _bookService = bookService;

    [BindProperty(SupportsGet = true)]
    public BookFilterDto Filter { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public PagedResultDto<BookDto> Books { get; set; } = new();

    public List<SelectListItem> ConditionOptions { get; } =
        Enum.GetValues<BookConditionEnum>()
            .Select(c => new SelectListItem(c.ToString(), c.ToString()))
            .ToList();

    public List<SelectListItem> LanguageOptions { get; } =
        Enum.GetValues<LanguageEnum>()
            .Select(l => new SelectListItem(l.ToString(), l.ToString()))
            .ToList();

    public async Task OnGetAsync()
    {
        var result = await _bookService.SearchAsync(Filter, PageNumber, PageSize);
        Books = result.Data ?? new PagedResultDto<BookDto>();
    }

    // Preserva todos os filtros ativos ao trocar de página, pra paginação
    // e filtro conviverem na mesma URL (GET) sem se perderem.
    public Dictionary<string, string> RouteValuesForPage(int page) => new()
    {
        ["PageNumber"] = page.ToString(),
        ["Filter.SearchText"] = Filter.SearchText ?? "",
        ["Filter.YearFrom"] = Filter.YearFrom?.ToString() ?? "",
        ["Filter.YearTo"] = Filter.YearTo?.ToString() ?? "",
        ["Filter.AddedFrom"] = Filter.AddedFrom?.ToString("yyyy-MM-dd") ?? "",
        ["Filter.AddedTo"] = Filter.AddedTo?.ToString("yyyy-MM-dd") ?? "",
        ["Filter.PagesFrom"] = Filter.PagesFrom?.ToString() ?? "",
        ["Filter.PagesTo"] = Filter.PagesTo?.ToString() ?? "",
        ["Filter.Language"] = Filter.Language?.ToString() ?? "",
        ["Filter.Condition"] = Filter.Condition?.ToString() ?? "",
        ["Filter.ValueFrom"] = Filter.ValueFrom?.ToString() ?? "",
        ["Filter.ValueTo"] = Filter.ValueTo?.ToString() ?? "",
            ["Filter.SortBy"] = Filter.SortBy.ToString(),
    };

    // Gera a lista de números de página no estilo "1 2 3 ... 8 9 10",
    // usando null como marcador de reticências.
    public List<int?> GetPageNumbers()
    {
        var total = Books.TotalPages;
        var current = Books.PageNumber;
        var pages = new List<int?>();

        if (total <= 0)
            return pages;

        // Poucas páginas: mostra todas, sem reticências.
        if (total <= (Window * 2) + 5)
        {
            for (var i = 1; i <= total; i++)
                pages.Add(i);
            return pages;
        }

        pages.Add(1);

        var start = Math.Max(2, current - Window);
        var end = Math.Min(total - 1, current + Window);

        if (start > 2)
            pages.Add(null);

        for (var i = start; i <= end; i++)
            pages.Add(i);

        if (end < total - 1)
            pages.Add(null);

        pages.Add(total);

        return pages;
    }
}