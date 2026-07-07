using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Comment;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Services;
using Bibliotheca.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Feed;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private const int CommentsPageSize = 10;

    private readonly IBookService _bookService;
    private readonly ICommentService _commentService;

    public DetailsModel(IBookService bookService, ICommentService commentService)
    {
        _bookService = bookService;
        _commentService = commentService;
    }

    public BookDto? Book { get; set; }
    public PagedResultDto<CommentDto> Comments { get; set; } = new();

    public string CanonicalSlug { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty]
    public string NewCommentContent { get; set; } = string.Empty;

    [BindProperty]
    public string NewCommentLink { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, string? slug)
    {
        var bookResult = await _bookService.GetByIdAsync(id);
        if (!bookResult.IsSuccess || bookResult.Data is null)
            return NotFound();

        Book = bookResult.Data;
        CanonicalSlug = SlugHelper.BuildBookSlug(Book.Name, Book.Author, Book.PublicationYear);

        if (!string.Equals(slug, CanonicalSlug, StringComparison.Ordinal))
        {
            return RedirectToPagePermanent(
                pageName: "/Feed/Details",
                routeValues: new { id, slug = CanonicalSlug });
        }

        await _bookService.RegisterViewAsync(id);

        var commentsResult = await _commentService.GetByBookIdPagedAsync(id, PageNumber, CommentsPageSize);
        Comments = commentsResult.Data ?? new PagedResultDto<CommentDto>();

        return Page();
    }

    public async Task<IActionResult> OnPostAddCommentAsync(Guid id, string? slug)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Challenge();

        if (string.IsNullOrWhiteSpace(NewCommentContent))
        {
            ErrorMessage = "Escreva um comentário antes de enviar.";
            return await OnGetAsync(id, slug);
        }

        var result = await _commentService.CreateAsync(new CreateCommentDto
        {
            UserId = userId,
            BookId = id,
            Content = NewCommentContent,
            Link = NewCommentLink
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return await OnGetAsync(id, slug);
        }

        return RedirectToPage(new { id, slug });
    }
}