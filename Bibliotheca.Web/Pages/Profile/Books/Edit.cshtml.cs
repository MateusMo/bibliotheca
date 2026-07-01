using System.Security.Claims;
using Bibliotheca.Application.Dtos.Author;
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
    private readonly IAuthorService _authorService;

    public EditModel(IBookService bookService, IAuthorService authorService)
    {
        _bookService = bookService;
        _authorService = authorService;
    }

    [BindProperty]
    public BookInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public List<AuthorDto> AvailableAuthors { get; set; } = [];

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
            AuthorIds = book.AuthorIds,
            IsOwner = book.IsOwner,
            PublicationYear = book.PublicationYear,
            PhotosText = string.Join(", ", book.Photos),
            Language = book.Language,
            Publisher = book.Publisher,
            ISBN = book.ISBN,
            Pages = book.Pages,
            Edition = book.Edition,
            EstimatedValue = book.EstimatedValue,
            Condition = book.Condition
        };

        await LoadAuthorsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadAuthorsAsync();

        // Reconfirma a posse antes de aceitar qualquer alteração
        // (evita editar livro de outro usuário trocando o id na URL).
        var existing = await _bookService.GetByIdAsync(Id);
        if (!existing.IsSuccess || existing.Data is null || existing.Data.UserId != CurrentUserId)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var result = await _bookService.UpdateAsync(new UpdateBookDto
        {
            Id = Id,
            AuthorIds = Input.AuthorIds,
            IsOwner = Input.IsOwner,
            Name = Input.Name,
            PublicationYear = Input.PublicationYear,
            Photos = Input.ParsePhotos(),
            Language = Input.Language,
            Publisher = Input.Publisher,
            ISBN = Input.ISBN,
            Pages = Input.Pages,
            Edition = Input.Edition,
            EstimatedValue = Input.EstimatedValue,
            Condition = Input.Condition
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        return RedirectToPage("/Profile/Index");
    }

    private async Task LoadAuthorsAsync()
    {
        var authorsResult = await _authorService.GetAllAsync();
        AvailableAuthors = authorsResult.Data ?? [];
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}