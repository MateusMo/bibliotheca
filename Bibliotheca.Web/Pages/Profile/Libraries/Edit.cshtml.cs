using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Library;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile.Libraries;

[Authorize]
public class EditModel : PageModel
{
    private readonly ILibraryService _libraryService;
    private readonly IBookService _bookService;

    public EditModel(ILibraryService libraryService, IBookService bookService)
    {
        _libraryService = libraryService;
        _bookService = bookService;
    }

    [BindProperty]
    public LibraryInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public List<BookDto> AvailableBooks { get; set; } = [];

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var libraryResult = await _libraryService.GetByIdAsync(Id);

        if (!libraryResult.IsSuccess || libraryResult.Data is null || libraryResult.Data.UserId != CurrentUserId)
            return NotFound();

        var library = libraryResult.Data;

        Input = new LibraryInputModel
        {
            Title = library.Title,
            Description = library.Description,
            SelectedBookIds = library.Books.Select(b => b.Id).ToList()
        };

        await LoadBooksAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var existing = await _libraryService.GetByIdAsync(Id);
        if (!existing.IsSuccess || existing.Data is null || existing.Data.UserId != CurrentUserId)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await LoadBooksAsync();
            return Page();
        }

        var result = await _libraryService.UpdateAsync(new UpdateLibraryDto
        {
            Id = Id,
            Title = Input.Title,
            Description = Input.Description ?? string.Empty,
            BookIds = Input.SelectedBookIds
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            await LoadBooksAsync();
            return Page();
        }

        return RedirectToPage("/Profile/Libraries/Index");
    }

    private async Task LoadBooksAsync()
    {
        var booksResult = await _bookService.GetByUserIdAsync(CurrentUserId);
        AvailableBooks = booksResult.Data ?? [];
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}