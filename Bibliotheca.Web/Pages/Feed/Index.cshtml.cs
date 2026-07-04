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
    private readonly IBookService _bookService;

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

    public Dictionary<string, string> RouteValuesForPage(int page) => new()
    {
        ["PageNumber"] = page.ToString(),
        ["Filter.Name"] = Filter.Name ?? "",
        ["Filter.AuthorName"] = Filter.AuthorName ?? "",
        ["Filter.Language"] = Filter.Language?.ToString() ?? "",
        ["Filter.YearFrom"] = Filter.YearFrom?.ToString() ?? "",
        ["Filter.YearTo"] = Filter.YearTo?.ToString() ?? ""
    };

    public async Task OnGetAsync()
    {
        var result = await _bookService.SearchAsync(Filter, PageNumber, PageSize);
        Books = result.Data ?? new PagedResultDto<BookDto>();
    }
}