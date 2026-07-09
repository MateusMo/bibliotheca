using System.Security.Claims;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Library;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Profile.Libraries;

[Authorize]
public class IndexModel : PageModel
{
    private const int PageSize = 10;

    private readonly ILibraryService _libraryService;

    public IndexModel(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public PagedResultDto<LibraryDto> Libraries { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var result = await _libraryService.DeleteAsync(id, CurrentUserId);

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            await LoadAsync();
            return Page();
        }

        return RedirectToPage(new { PageNumber });
    }

    private async Task LoadAsync()
    {
        var result = await _libraryService.GetByUserIdPagedAsync(CurrentUserId, PageNumber, PageSize);
        Libraries = result.Data ?? new PagedResultDto<LibraryDto>();
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}