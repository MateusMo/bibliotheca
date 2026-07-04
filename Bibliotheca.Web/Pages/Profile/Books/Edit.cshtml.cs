using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile.Books;

[Authorize]
public class EditModel : PageModel
{
    private readonly IBookService _bookService;

    public EditModel(IBookService bookService)
    {
        _bookService = bookService;
    }

    [BindProperty]
    public BookInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var bookResult = await _bookService.GetByIdAsync(Id);

        if (!bookResult.IsSuccess || bookResult.Data is null || bookResult.Data.UserId != CurrentUserId)
            return NotFound();

        var book = bookResult.Data;

        Input = new BookInputModel
        {
            Name = book.Name,
            Author = book.Author,
            Description = book.Description,
            IsOwner = book.IsOwner,
            PublicationYear = book.PublicationYear,
            PhotosText = string.Join(", ", book.Photos),
            LanguageEnum = book.LanguageEnum,
            Publisher = book.Publisher,
            ISBN = book.ISBN,
            Pages = book.Pages,
            EstimatedValue = book.EstimatedValue,
            ConditionEnum = book.ConditionEnum
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var existing = await _bookService.GetByIdAsync(Id);
        if (!existing.IsSuccess || existing.Data is null || existing.Data.UserId != CurrentUserId)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var result = await _bookService.UpdateAsync(new UpdateBookDto
        {
            Id = Id,
            IsOwner = Input.IsOwner,
            Name = Input.Name,
            Author = Input.Author,
            Description = Input.Description ?? string.Empty,
            PublicationYear = Input.PublicationYear,
            Photos = Input.ParsePhotos(),
            LanguageEnum = Input.LanguageEnum,
            Publisher = Input.Publisher,
            ISBN = Input.ISBN,
            Pages = Input.Pages,
            EstimatedValue = Input.EstimatedValue,
            ConditionEnum = Input.ConditionEnum
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        return RedirectToPage("/Profile/Index");
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}