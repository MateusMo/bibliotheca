using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Bibliotheca.Application.Dtos.Author;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Services;
using Bibliotheca.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile.Books;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;

    public CreateModel(IBookService bookService, IAuthorService authorService)
    {
        _bookService = bookService;
        _authorService = authorService;
    }

    [BindProperty]
    public BookInputModel Input { get; set; } = new();

    public List<AuthorDto> AvailableAuthors { get; set; } = [];

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAuthorsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadAuthorsAsync();

        if (!ModelState.IsValid)
            return Page();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _bookService.CreateAsync(new CreateBookDto
        {
            UserId = userId,
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
}

// Compartilhado entre Create e Edit (mesmo namespace).
public class BookInputModel
{
    [Required(ErrorMessage = "Informe o título")]
    [StringLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione ao menos um autor")]
    [MinLength(1, ErrorMessage = "Selecione ao menos um autor")]
    public List<Guid> AuthorIds { get; set; } = [];

    public bool IsOwner { get; set; } = true;

    [Range(0, 9999, ErrorMessage = "Ano inválido")]
    public int PublicationYear { get; set; } = DateTime.UtcNow.Year;

    // Fotos guardadas só como texto/URLs por enquanto.
    // Upload real (Cloudflare R2) fica para uma próxima sessão.
    [Display(Name = "Fotos (URLs separadas por vírgula)")]
    public string? PhotosText { get; set; }

    [Required(ErrorMessage = "Informe o idioma")]
    [StringLength(50)]
    public string Language { get; set; } = string.Empty;

    [StringLength(200)]
    public string Publisher { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o ISBN")]
    [StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [Range(1, 100000, ErrorMessage = "Número de páginas inválido")]
    public int Pages { get; set; }

    [StringLength(50)]
    public string Edition { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Valor inválido")]
    public decimal EstimatedValue { get; set; }

    [Required]
    public BookCondition Condition { get; set; } = BookCondition.Good;

    public string[] ParsePhotos()
        => string.IsNullOrWhiteSpace(PhotosText)
            ? []
            : PhotosText.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}