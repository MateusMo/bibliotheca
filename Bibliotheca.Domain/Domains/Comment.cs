namespace Bibliotheca.Domain.Domains;

public class Comment : AbstractBase
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public Book Book { get; set; } = null!;
}