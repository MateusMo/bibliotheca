using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Services;
using Bibliotheca.Domain.Enums;
using Bibliotheca.Domain.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile.Books;

[Authorize]
public class EditModel : PageModel
{
    private readonly IBookService _bookService;

    private readonly IUserService _userService;
    
    public EditModel(IBookService bookService,IUserService userService)
    {
        _bookService = bookService;
        _userService = userService;
    }

    [BindProperty]
    public BookInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public string? ErrorMessage { get; set; }
    public int MaxPhotos { get; set; }
    

    public async Task<IActionResult> OnGetAsync()
    {
        var bookResult = await _bookService.GetByIdAsync(Id);
        
        await LoadMaxPhotosAsync();
        
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

    private async Task LoadMaxPhotosAsync()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userResult = await _userService.GetByIdAsync(userId);
        MaxPhotos = (userResult.Data?.PlanType ?? PlanTypeEnum.Free).MaxPhotosPerBook();
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