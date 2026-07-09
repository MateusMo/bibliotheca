namespace Bibliotheca.Domain.Domains;

public class Library : AbstractBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public User User { get; set; } = null!;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}