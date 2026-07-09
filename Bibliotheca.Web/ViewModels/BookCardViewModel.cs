namespace Bibliotheca.Web.ViewModels;

public class BookCardViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ProfileScore { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int PhotoCount { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public long ViewCount { get; set; }
    public string Slug { get; set; } = string.Empty;
}