using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

    public CreateModel(IBookService bookService)
    {
        _bookService = bookService;
    }

    [BindProperty]
    public BookInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _bookService.CreateAsync(new CreateBookDto
        {
            UserId = userId,
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
}

// Compartilhado entre Create e Edit (mesmo namespace).
public class BookInputModel
{
    [Required(ErrorMessage = "Informe o título")]
    [StringLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o autor")]
    [StringLength(300)]
    public string Author { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    public bool IsOwner { get; set; } = true;

    [Range(0, 9999, ErrorMessage = "Ano inválido")]
    public int PublicationYear { get; set; } = DateTime.UtcNow.Year;

    [Display(Name = "Fotos (URLs separadas por vírgula)")]
    public string? PhotosText { get; set; }

    [Required(ErrorMessage = "Selecione o idioma")]
    public LanguageEnum LanguageEnum { get; set; } = LanguageEnum.Portuguese;

    [StringLength(200)]
    public string Publisher { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o ISBN")]
    [StringLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [Range(1, 100000, ErrorMessage = "Número de páginas inválido")]
    public int Pages { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Valor inválido")]
    public decimal EstimatedValue { get; set; }

    [Required]
    public BookConditionEnum ConditionEnum { get; set; } = BookConditionEnum.Good;

    public string[] ParsePhotos()
        => string.IsNullOrWhiteSpace(PhotosText)
            ? []
            : PhotosText.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}