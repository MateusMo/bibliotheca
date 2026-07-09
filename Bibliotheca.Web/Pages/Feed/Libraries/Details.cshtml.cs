using Bibliotheca.Application.Dtos.Library;
using Bibliotheca.Application.Services;
using Bibliotheca.Web.Utils;
using Bibliotheca.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Feed.Libraries;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly ILibraryService _libraryService;

    public DetailsModel(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public LibraryDto? Library { get; set; }
    public string CanonicalSlug { get; set; } = string.Empty;
    public List<BookCardViewModel> Books { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id, string? slug)
    {
        var result = await _libraryService.GetByIdAsync(id);
        if (!result.IsSuccess || result.Data is null)
            return NotFound();

        Library = result.Data;
        CanonicalSlug = SlugHelper.BuildLibrarySlug(Library.Title);

        if (!string.Equals(slug, CanonicalSlug, StringComparison.Ordinal))
        {
            return RedirectToPagePermanent(
                pageName: "/Feed/Libraries/Details",
                routeValues: new { id, slug = CanonicalSlug });
        }

        Books = Library.Books.Select(b => new BookCardViewModel
        {
            Id = b.Id,
            Name = b.Name,
            Author = b.Author,
            PublicationYear = b.PublicationYear,
            UserName = Library.UserName,
            ProfileScore = Library.ProfileScore,
            CreatedAt = b.CreatedAt,
            PhotoCount = b.Photos.Length,
            Language = b.LanguageEnum.ToString(),
            Condition = b.ConditionEnum.ToString(),
            ViewCount = b.ViewCount,
            Slug = SlugHelper.BuildBookSlug(b.Name, b.Author, b.PublicationYear)
        }).ToList();

        return Page();
    }
}