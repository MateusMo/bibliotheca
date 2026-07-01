using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Dtos.Comment;

public class CommentDto : BaseDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid BookId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}