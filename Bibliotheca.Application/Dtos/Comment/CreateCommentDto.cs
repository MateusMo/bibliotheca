// Bibliotheca.Application/Dtos/Comment/CreateCommentDto.cs
namespace Bibliotheca.Application.Dtos.Comment;

public class CreateCommentDto
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}