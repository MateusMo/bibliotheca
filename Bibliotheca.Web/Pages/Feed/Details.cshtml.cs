using System.Security.Claims;
using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Comment;
using Bibliotheca.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bibliotheca.Web.Pages.Feed;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly IBookService _bookService;
    private readonly ICommentService _commentService;

    public DetailsModel(IBookService bookService, ICommentService commentService)
    {
        _bookService = bookService;
        _commentService = commentService;
    }

    public BookDto? Book { get; set; }
    public List<CommentDto> Comments { get; set; } = [];

    [BindProperty]
    public string NewCommentContent { get; set; } = string.Empty;

    [BindProperty]
    public string NewCommentLink { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var bookResult = await _bookService.GetByIdAsync(id);
        if (!bookResult.IsSuccess || bookResult.Data is null)
            return NotFound();

        Book = bookResult.Data;

        var commentsResult = await _commentService.GetByBookIdAsync(id);
        Comments = commentsResult.Data ?? [];

        return Page();
    }

    // Create de comentário exige usuário logado, validado pelo cookie de autenticação.
    public async Task<IActionResult> OnPostAddCommentAsync(Guid id)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Challenge();

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Challenge();

        if (string.IsNullOrWhiteSpace(NewCommentContent))
        {
            ErrorMessage = "Escreva um comentário antes de enviar.";
            return await OnGetAsync(id);
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
            return await OnGetAsync(id);
        }

        return RedirectToPage(new { id });
    }
}