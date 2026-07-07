using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.ProfileScore;
using Bibliotheca.Application.Dtos.User;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile;

[Authorize]
public class IndexModel : PageModel
{
    private const int PageSize = 10;

    private readonly IUserService _userService;
    private readonly IBookService _bookService;
    private readonly IProfileScoreService _profileScoreService;

    public IndexModel(IUserService userService, IBookService bookService, IProfileScoreService profileScoreService)
    {
        _userService = userService;
        _bookService = bookService;
        _profileScoreService = profileScoreService;
    }

    public UserDto? Account { get; set; }
    public PagedResultDto<BookDto> Books { get; set; } = new();
    public ProfileScoreDto? Score { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty]
    public ProfileNameInputModel NameInput { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostUpdateNameAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var current = await _userService.GetByIdAsync(CurrentUserId);
        if (!current.IsSuccess || current.Data is null)
            return NotFound();

        var result = await _userService.UpdateAsync(new UpdateUserDto
        {
            Id = CurrentUserId,
            Name = NameInput.Name,
            Email = current.Data.Email
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            await LoadAsync();
            return Page();
        }

        SuccessMessage = "Nome atualizado.";
        return RedirectToPage(new { PageNumber });
    }

    public async Task<IActionResult> OnPostDeleteBookAsync(Guid id)
    {
        var bookResult = await _bookService.GetByIdAsync(id);

        if (!bookResult.IsSuccess || bookResult.Data is null || bookResult.Data.UserId != CurrentUserId)
        {
            ErrorMessage = "Livro não encontrado ou você não tem permissão para excluí-lo.";
            await LoadAsync();
            return Page();
        }

        await _bookService.DeleteAsync(id);
        return RedirectToPage(new { PageNumber });
    }

    private async Task LoadAsync()
    {
        var userResult = await _userService.GetByIdAsync(CurrentUserId);
        Account = userResult.Data;
        NameInput.Name = Account?.Name ?? string.Empty;

        var booksResult = await _bookService.GetByUserIdPagedAsync(CurrentUserId, PageNumber, PageSize);
        Books = booksResult.Data ?? new PagedResultDto<BookDto>();

        var scoreResult = await _profileScoreService.GetByUserIdAsync(CurrentUserId);
        Score = scoreResult.Data;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public class ProfileNameInputModel
{
    [Required(ErrorMessage = "Informe o nome")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
}