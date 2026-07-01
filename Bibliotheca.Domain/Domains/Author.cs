namespace Bibliotheca.Domain.Domains;

public class Author : AbstractBase
{
    public string Photo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDay { get; set; }
    public DateTime? DeathDay { get; set; } 
    public string Description { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}