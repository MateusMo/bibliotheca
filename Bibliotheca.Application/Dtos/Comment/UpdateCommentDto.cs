// Bibliotheca.Application/Dtos/Comment/UpdateCommentDto.cs
namespace Bibliotheca.Application.Dtos.Comment;

public class UpdateCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}