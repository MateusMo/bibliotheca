using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Library;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile.Libraries;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ILibraryService _libraryService;
    private readonly IBookService _bookService;

    public CreateModel(ILibraryService libraryService, IBookService bookService)
    {
        _libraryService = libraryService;
        _bookService = bookService;
    }

    [BindProperty]
    public LibraryInputModel Input { get; set; } = new();

    public List<BookDto> AvailableBooks { get; set; } = [];

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadBooksAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadBooksAsync();
            return Page();
        }

        var result = await _libraryService.CreateAsync(new CreateLibraryDto
        {
            UserId = CurrentUserId,
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

// Compartilhado entre Create e Edit (mesmo namespace).
public class LibraryInputModel
{
    [Required(ErrorMessage = "Informe o título")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public List<Guid> SelectedBookIds { get; set; } = [];
}