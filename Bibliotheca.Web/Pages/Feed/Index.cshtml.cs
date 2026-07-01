using Bibliotheca.Application.Dtos.Book;
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
    private readonly IBookService _bookService;

    public IndexModel(IBookService bookService)
    {
        _bookService = bookService;
    }

    // SupportsGet = true: o filtro fica na querystring, dá pra
    // compartilhar o link já filtrado e o botão "voltar" funciona.
    [BindProperty(SupportsGet = true)]
    public BookFilterDto Filter { get; set; } = new();

    public List<BookDto> Books { get; set; } = [];

    public List<SelectListItem> ConditionOptions { get; } =
        Enum.GetValues<BookCondition>()
            .Select(c => new SelectListItem(c.ToString(), c.ToString()))
            .ToList();

    public async Task OnGetAsync()
    {
        var result = await _bookService.SearchAsync(Filter);
        Books = result.Data ?? [];
    }
}